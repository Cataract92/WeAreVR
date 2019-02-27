# Bugs
* Shaders
* Hand Modell Position beim greifen
* Flippen beim Plunger

Requirements: SteamVR vom AssetStore


Objekte manipulieren: 

  1. Mit einer Hand greifen und drehen
  2. Ecken/Kanten/Flächen mit einer Hand greifen und ziehen/verformen
  3. Mit zwei Händen greifen und skalieren
                      
Raster für Ecken und Objekt-Interaktion

Nutzer skalieren -> Blickwinkel ändern um die Welt zu vergrößer/verkleinern

# Hauptpunkte:
* Ecken auf Flächen erzeugen (An Position des Controllers)
* Ecken löschen
* Ecken ziehen
* Kanten ziehen => Zwei Ecken ziehen
* Flächen ziehen => Vier Ecken ziehen
* Translation/Skalierung/Rotation
* Skalierung des Spielers

# Implementierung:
* Translation/Skalierung/Rotation auf Grip-Button
* Modus auf Trackpad umschalten (Verformen, Erzeugen, Farben?)
* Post-Filiale-Algorithmus (Patrik implementiert den)


# Wishlist:
* Ecken an Kanten ziehen/erzeugen
* Objekte kombinieren
* Texturen / Farben
* Parallele Flächen wieder zusammen fügen, und ganze Flächen ziehen

# Zeitplan:
| KW     | Ziel    |
| --------|---------|
| 4 | Einleitung/Besprechung/Planung |
| 5 | Greifen/Rotation (Grip Funktionen), Post-Algorithm |
| 6 | Ecken Ziehen, Datenstruktur |
| 7 | Modi auswählen, Zwischenpräsentation orbereiten/halten |
| 8 | Kanten/Flächen Ziehen |
| 9 | Ecken hinzufügen, Skalierung (Spieler) |
| 10 | Ecken Löschen |
