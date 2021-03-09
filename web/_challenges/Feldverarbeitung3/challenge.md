---
author: 1e3fcd02b1547f847cb7fc3add4484a5
title: Verändertes Array
ratingMethod: Fixed
category: Grundlagen
stickAsBeginner: false
date: 2019-06-21T14:34:59.0724847+02:00
source: none
learningFocus: 
isDraft: false
includeTests: []
dependsOn: []
languages: []
state:
  passedCount: 33
  failedCount: 57
  hasError: false
  errorDescription: ''
  lastEditorId: 
  feasibilityIndex: 580
  feasibilityIndexMod: 2
  difficultyRating: 60
  isPartOfBundle: true
  minEffort: 30 mins
  maxEffort: 8 hrs
  features:
  - Arrays
  - Iterations
  activity: -14568
lastEdit: 2019-06-21T14:34:59.0724847+02:00

---
In dieser Aufgabe werden einfache Operationen auf Arrays geübt. Lesen Sie dazu zunächst
ein Array a vom Benutzer ein. Der Benutzer soll dazu nach einer gewünschten Größe n
für das Array gefragt werden und anschließend nacheinander die Elemente des Arrays
eingeben. Das Array muss mindestens 2 Elemente enthalten; gibt die Benutzerin eine
kleinere Größe ein, soll sie auf ihren Irrtum hingewiesen und um erneute Eingabe gebeten
werden. Nutzen Sie diesen Teil des Programms für die folgenden Teilaufgaben.

Schreiben Sie ein Programm, welches jeweils zwei benachbarte Elemente addiert
und das erste mit der Summe überschreibt. Hat das Array eine ungerade Anzahl
Elemente, so bleibt am Ende des Arrays ein einzelnes Element übrig. Dieses soll
unverändert bleiben. Das Ergebnis-Feld soll ausgegeben werden.

{% output %}
> array.exe
Wieviele Elemente soll das Array beinhalten: `1`
Bitte geben Sie eine Zahl größer 1 ein!
Wieviele Elemente soll das Array beinhalten: `4`
Geben Sie nun die Elemente der Reihenfolge nach ein:
`4`
`3`
`7`
`1`

Ergebnis:
[7, 3, 8, 1]
{% endoutput %}