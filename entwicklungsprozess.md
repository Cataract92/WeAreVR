Probleme die aufgetreten sind bei der Entwicklung

# Spielerskalierung
* Spieler Scale lief dauernd auf 0
* Skalierung lief über Hand Distanz, aber sobald die Größe sich veränderte, änderte sich der Wert exponentiell
* Größe sprung hin und her gegen 1
* Gingen zurück auf die Ursprungsversion und haben die änderungen gedämpft
* Methode set hat nicht funktioniert, führte dazu dass wir anderes im Verdacht hatten

## Formel Varianten
* Player.transform.localscale \*= newDistance / initialdistance
* Player.transform.localscale \*= newDistance / (initialdistance \* Player.transform.localScale)
* Player.transform.localscale \*= newDistance / initialdistance
* Player.transform.localscale \*= ((newDistance / initialdistance) + 1) / 2

## Tools
Beim Skalierung wurden die Tools am Gürtel auch zu groß, und haben das sehen beeinträchtigt

Erst haben wir Vektoren von der Camera aus die Position der Tools skalieren können mit dem Skalierungsfaktor des Speilers.

Dann war das Problem dass die Tools auch an die Rotation des Kopfs des Spielers gebunden war, und damit nicht an dem "Gürtel" des Spielers fest hingen.



# Ecken Ziehen
Did some things, changed some stuff

# Flächen Ziehen


# Objekt Skalieren

Während dem Skalieren wurden die Positionen der Ecken des Mesches falsch berechnet, da die Skalierung des Objekts nicht brücksichtigt wurde

# Ecken Erzeugen

Ecken wurden 180° von Spieler weg erzeugt

Das blaue VertexDummy auf der Fläche wurde gelöscht und alles hing sich auf -> 
1. 
1. Umfärben auf Rot und weiter verwenden als Ecken DummyVertex


1. Man kann Ecken erzeugen, sie werden aber nicht anschließend an die Hand gebunden um gezogen zu werden
2. Dann haben wir das Ziehen der Ecken kaputt gemacht
3.

# Neue Ecken Kreieren

Tool wurde hinzugefügt, war zu groß, wurde verkleinert

Tool ist nicht richtig in der Hand erschienen, hat Fehler geworfen

# Neue Objekte

Größe war scheiße

Interagieren mit Objekten hat andere Objekte kaputt gemacht
- Ursach war im Erzeugen der neuen Ecken und wie die Objekte gesucht werden 

# Ecken Löschen

Bei der uhrsrünglichen Idee(2d Objekt Punkt in Mitte, von einem Punkt an alle anderen, Faces Ziehen): Zu viele Faces wurden gezogen, auch mitten durch das Objekt durch.

Lösungsansatz: Ein rekursiver Algorithmus der alle Möglichkeiten nach gültigkeit überprüft, bis er ein valides Mesh findet.

Kanten wurden nicht immer richtig rum gemahlt
Ansätze:
1. Normal Ray Vergleich
1. Schnitte Zählen (ob gerade oder ungerade)

Lösung: von beiden Seiten zeichnen - Dunkelheit hat gesiegt also nvm

# Objekte Löschen






