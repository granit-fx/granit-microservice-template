#!/usr/bin/env bash
# Regenerate all EF Core migrations from scratch.
#
# Deletes every existing migration .cs file (including ModelSnapshot) and
# re-runs `dotnet ef migrations add Init` for each DbContext below. Run this
# whenever the model drifts beyond what incremental migrations can cleanly
# express — typically after a Granit / EF Core / Wolverine package bump — and
# you want a single consolidated Init per service.
#
# Each microservice references Microsoft.EntityFrameworkCore.Design and owns
# its IDesignTimeDbContextFactory, so every project is its own --startup-project.
# No database connection is required: `migrations add` works fully offline.
#
# Requirements:
#   - dotnet-ef installed (`dotnet tool install --global dotnet-ef`)
#
# Usage: scripts/regenerate-migrations.sh
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

# Format: project_path | DbContext class | output_dir (relative to project)
CONTEXTS=(
  "src/GranitMicroservice.CatalogService|CatalogDbContext|Persistence/Migrations"
  "src/GranitMicroservice.NotificationService|NotificationServiceDbContext|Persistence/Migrations"
  "src/GranitMicroservice.IdentityService|IdentityServiceDbContext|Persistence/Migrations"
)

echo ">> Deleting existing migration files..."
for entry in "${CONTEXTS[@]}"; do
  IFS='|' read -r proj _ctx out <<< "$entry"
  dir="$ROOT/$proj/$out"
  if [ -d "$dir" ]; then
    # Remove all .cs files (Init, Designer, ModelSnapshot) but keep the directory
    find "$dir" -maxdepth 1 -type f -name '*.cs' -delete
    echo "   cleaned $proj/$out"
  fi
done

echo ">> Building solution (once)..."
# --no-incremental: force a full recompile so the subsequent `migrations add --no-build`
# never loads stale assemblies (e.g. after a NuGet version bump), which would silently
# emit migrations against the previous package set.
dotnet build GranitMicroservice.slnx -c Debug -v quiet -clp:ErrorsOnly --no-incremental

echo ">> Regenerating migrations..."
for entry in "${CONTEXTS[@]}"; do
  IFS='|' read -r proj ctx out <<< "$entry"
  echo "   → $ctx  ($proj → $out)"
  dotnet ef migrations add Init \
    --project "$proj" \
    --startup-project "$proj" \
    --context "$ctx" \
    --output-dir "$out" \
    --no-build
done

echo ">> All migrations regenerated."
