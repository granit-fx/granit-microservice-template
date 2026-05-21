#!/usr/bin/env bash
# Kills leftover AppHost / service / DCP processes after a non-graceful shutdown.
# Safe to run anytime — only matches processes started by this template.
#
# Usage:
#   scripts/reset.sh              # kill orphans, report
#   scripts/reset.sh --ports      # also list who holds our dev ports
#   scripts/reset.sh --hard       # SIGKILL instead of SIGTERM
set -euo pipefail

PORTS=(5100 5110 5120 5130)
SIGNAL="-TERM"
SHOW_PORTS=0

for arg in "$@"; do
    case "$arg" in
        --hard) SIGNAL="-KILL" ;;
        --ports) SHOW_PORTS=1 ;;
        -h|--help)
            sed -n '2,8p' "$0"
            exit 0
            ;;
        *)
            echo "unknown arg: $arg" >&2
            exit 2
            ;;
    esac
done

kill_pattern() {
    local label=$1 pattern=$2
    local pids
    pids=$(pgrep -f "$pattern" 2>/dev/null || true)
    if [[ -z "$pids" ]]; then
        echo "  $label: none"
        return
    fi
    echo "  $label: killing $(echo "$pids" | tr '\n' ' ')"
    # shellcheck disable=SC2086
    kill "$SIGNAL" $pids 2>/dev/null || true
}

echo "Reset: stopping leftover Granit microservice template processes…"
kill_pattern "AppHost"            'GranitMicroservice\.AppHost'
kill_pattern "Identity service"   'GranitMicroservice\.IdentityService'
kill_pattern "Catalog service"    'GranitMicroservice\.CatalogService'
kill_pattern "Notification svc"   'GranitMicroservice\.NotificationService'
kill_pattern "API gateway"        'GranitMicroservice\.ApiGateway'
kill_pattern "Aspire DCP host"    '[Aa]spire\.Hosting\.AppHost'
kill_pattern "DCP runtime"        '(^|/)dcp( |$)'
kill_pattern "DCP controller"     'dcpctrl'

# Brief settle so sockets release before next bind.
sleep 1

if (( SHOW_PORTS )); then
    echo ""
    echo "Port holders (empty = free):"
    for port in "${PORTS[@]}"; do
        holder=$(ss -tlnp 2>/dev/null | awk -v p=":$port" '$4 ~ p {print $0}')
        if [[ -z "$holder" ]]; then
            echo "  :$port  free"
        else
            echo "  :$port  $holder"
        fi
    done
fi

echo "Done."
