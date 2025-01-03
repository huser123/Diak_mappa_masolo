# MappaMásoló

## Leírás
A MappaMásoló egy Windows alatti WPF alkalmazás, amely diákokhoz tartozó mappák tartalmát másolja egy forrás könyvtárból egy cél könyvtárba. Az alkalmazás főként osztályok és diákok adatainak kezelésére és másolására szolgál.

---

## Működés
1. **Névsort tartalmazó fájl (NEVSOR.txt)**
   Az alkalmazás a `NEVSOR.txt` fájlból tölti be az osztályok és diákok adatait a fordításkor. Utána egy módosítás esetén újra kell fordítani a fájlt az új adatokkal.
   
   A fájl formátuma:
   ```
   Osztály: 09A
   Útvonal: T:\elérési út\09A
   Kovács Anna
   Nagy Béla
   Szabó Csilla
   
   Osztály: 10A
   Útvonal: T:\elérési út\10A
   Tóth Dávid
   Farkas Eszter
   Horváth Gergő
   ```
   - **Osztály**: Az osztály neve.
   - **Útvonal**: Az osztály forrás könyvtára.
   - **Diákok**: Az osztályhoz tartozó diákok nevei soronként.

2. **Mappanév generálás**
   - Az alkalmazás a diáknevekből automatikusan generálja a mappaneveket az alábbi szabályok szerint:
     - Az ékezetes betűk ékezet nélküli változatra cserélődnek (pl. `Á` → `A`).
     - A szóköz aláhúzássá alakul (pl. `Kovács Anna` → `Kovacs_Anna`).
     - Az olyan mappák is kezelve vannak, amelyek számmal végződnek (pl. `Kovacs_Anna2`).

3. **Forrás és cél könyvtár**
   - **Forrás könyvtár**: A `NEVSOR.txt`-ben megadott útvonalak alapján.
   - **Cél könyvtár**: Az alkalmazás a `Z:\elérési út\célhely` könyvtárat használja alapértelmezett célként. Ez a könyvtár módosítható a kódban a `alapCelKonyvtar` változó értékének megváltoztatásával:
     ```csharp
     private readonly string alapCelKonyvtar = "Z:\\elérési út\\célhely";
     ```

4. **Másolási folyamat**
   - Az osztály kiválasztása után megjelennek a diákok nevei pipálható formában.
   - Az "Indítás" gomb elindítja a másolási folyamatot a kijelölt diákok mappáinak tartalmával.
   - A másolási folyamat megszakítható a "Megszakítás" gombbal.
   - A másolás során egy progress bar és egy szöveges kijelző mutatja az aktuális állapotot és a másolt fájl nevét.

---

## Beállítások és testreszabás
- **Forrás könyvtárak**: A `NEVSOR.txt` fájlban szükséges megadni az osztályok forrás útvonalait.
- **Cél könyvtár**: A célkönyvtár alapértelmezett értékét a `alapCelKonyvtar` változó határozza meg a kódban.
- **NEVSOR.txt fájl helye**: A `NEVSOR.txt` fájlt a projekt mappájában kell elhelyezni, a következő beállításokkal:
  - **Build Action**: `Embedded Resource`
  - **Build Action**: `Content` (az .old esetén)
  - **Copy to Output Directory**: `Copy always`

---

## Fordítás
Minden szükséges fájlt és beállítást **fordítás előtt** kell elvégezni:
1. Állítsd be a cél könyvtárat a kódban, ha szükséges.
2. Helyezd el a `NEVSOR.txt` fájlt a projekt fő mappájában a megadott formátumban és beállításokkal.
3. Győződj meg róla, hogy a fájlok és könyvtárak megfelelnek az elvárt struktúrának.

---

## Futtatás (fordítás után)
2. Indítsd el az alkalmazást.
3. Válaszd ki az osztályt és a másolandó diákokat.
4. Kattints az "Indítás" gombra.

---

## Korlátok
- Az alkalmazás kizárólag Windows alatt futtatható.
- A `NEVSOR.txt` fájl formátumának pontos betartása szükséges.
