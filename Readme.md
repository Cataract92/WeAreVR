Requirements: SteamVR vom AssetStore


Objekte manipulieren: 1. Mit einer Hand greifen und drehen
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

# Implementierung
* Translation/Skalierung/Rotation auf Grip-Button
* Modus auf Trackpad umschalten (Verformen, Erzeugen, Farben?)
* Post-Filiale-Algorithmus (Patrik implementiert den)


# Wishlist:
* Ecken an Kanten ziehen/erzeugen
* Ohne Toggle Translation/Skalierung/Rotation
* Objekte kombinieren
* Runde Objekte
* 2x Einhörner oder 1x Zweihörner
* Regenbögen
* Texturen / Farben
