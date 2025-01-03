#!/bin/bash

# Mappa bejárása
for dir in */; do
    # Eredeti név
    original_name="$dir"
    # Ékezetes karakterek eltávolítása, szóköz cseréje aláhúzásra
    new_name=$(echo "$original_name" | sed 'y/áÁéÉíÍóÓöÖőŐúÚüÜűŰ/aAeEiIoOoOoOuUuUuU/; s/ /_/g' | sed 's:/$::')
    # Átnevezés, ha szükséges
    if [ "$original_name" != "$new_name" ]; then
        mv "$original_name" "$new_name"
        echo "Átnevezve: $original_name -> $new_name"
    fi
done
