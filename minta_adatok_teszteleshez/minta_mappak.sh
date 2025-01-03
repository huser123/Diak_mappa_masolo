#!/bin/bash

# Fő mappa létrehozása
MAIN_FOLDER="09A"
mkdir -p "$MAIN_FOLDER"

# Véletlenszerű nevek listája (Vezetéknév Keresztnév formában)
# Ezek a nevek Véletlenszerűen generáltak. Nem valódi diáknévsor.

# 09A
#: <<'BLOCK'
NEVEK=(
"Kovács Anna"
"Nagy Béla"
"Szabó Csilla"
"Varga Dávid"
"Tóth Eszter"
"Kiss Péter"
"Farkas Ákos"
"Horváth Zsófia"
"Molnár Gábor"
"Németh Éva"
"Balogh István"
"Simon Katalin"
"Lukács Gergely"
"Juhász Rita"
"Fazekas András"
"Veres Noémi"
"Pintér Márk"
)
#BLOCK

# 10A
#: <<'BLOCK'
NEVEK=(
"Tóth Dávid"
"Farkas Eszter"
"Horváth Gergő"
"Kiss Csaba"
"Szabó Judit"
"Varga Krisztián"
"Kovács Zoltán"
"Molnár Nóra"
"Balogh Tamás"
"Nagy Anikó"
"Németh László"
"Varga Ágnes"
"Fazekas Dorottya"
"Simon Ádám"
"Lukács Lilla"
"Juhász Zsolt"
"Veres Kinga"
)
#BLOCK

# 11B
: <<'BLOCK'
NEVEK=(
"Molnár Ivett"
"Kiss János"
"Lakatos Péter"
"Kovács Zsuzsa"
"Nagy Viktor"
"Szabó Fanni"
"Varga Mihály"
"Tóth Enikő"
"Balogh Ádám"
"Farkas Vivien"
"Horváth Norbert"
"Németh Júlia"
"Simon Róbert"
"Juhász Gabriella"
"Lukács Levente"
"Fazekas Hanna"
"Veres Dominik"
)
BLOCK

# Elérhető kiterjesztések listája
KITERJESZTESEK=("txt" "csv" "log" "docx" "pptx" "html" "xml" "json" "xlsx")


# Mappák és fájlok generálása
for NEV in "${NEVEK[@]}"; do
  # Mappa létrehozása
  mkdir -p "$MAIN_FOLDER/$NEV"

  # Belépés a mappába
  cd "$MAIN_FOLDER/$NEV"

  # 15 fájl generálása
  for i in {1..15}; do
    ## Véletlenszerű fájlméret 1MB és 5MB között
    # FILE_SIZE=$((1024 * 1024 * (RANDOM % 5 + 1)))

    # Véletlenszerű fájlméret 1KB és 5KB között
    FILE_SIZE=$((1024 * (RANDOM % 5 + 1)))

    # Véletlenszerű kiterjesztés kiválasztása
    KITERJESZTES=${KITERJESZTESEK[$RANDOM % ${#KITERJESZTESEK[@]}]}

    # Fájlnév generálása
    FILE_NAME="file-$(printf "%04x" $RANDOM).$KITERJESZTES"

    # Fájl generálása
    head -c "$FILE_SIZE" /dev/urandom > "$FILE_NAME"
  done

  # Visszalépés a fő mappába
  cd - > /dev/null
done

echo "A mappák és fájlok sikeresen létre lettek hozva a(z) $MAIN_FOLDER könyvtárban."
