---
author: 1e3fcd02b1547f847cb7fc3add4484a5
title: Rechnen mit Arrays
ratingMethod: Fixed
category: Grundlagen
stickAsBeginner: false
date: 2019-06-21T13:55:23.2327400+02:00
source: none
learningFocus: 
isDraft: false
includeTests: []
dependsOn: []
languages: []
state:
  passedCount: 97
  failedCount: 111
  hasError: false
  errorDescription: ''
  lastEditorId: 
  feasibilityIndex: 873
  feasibilityIndexMod: 0
  difficultyRating: 60
  isPartOfBundle: true
  minEffort: 30 mins
  maxEffort: 8 hrs
  features:
  - Arrays
  - Iterations
  activity: -14568
lastEdit: 2021-01-25T10:49:13.6932658+01:00

---
In dieser Aufgabe werden einfache Operationen auf Arrays geübt. Lese dazu zunächst
ein Array `a` vom Benutzer ein. Der Benutzer soll zuerst nach einer gewünschten Größe `n`
für das Array gefragt werden und anschließend nacheinander die Elemente des Arrays
eingeben. Das Array muss mindestens `2` Elemente enthalten; gibt der Benutzer eine
kleinere Größe ein, soll er auf seinen Irrtum hingewiesen und um erneute Eingabe gebeten
werden. 

Nach dem Einlesen sollen alle Elemente an geraden Positionen (=Indices) addiert und
von der Summe der Elemente an ungeraden Positionen subtrahiert werden. Beachte dabei, dass bei Arrays der erste Indice bei `0` (=gerade) beginnt.

{% output %}
> array.exe
Wieviele Elemente soll das Array beinhalten: `1`
Bitte geben Sie eine Zahl größer 1 ein!
Wieviele Elemente soll das Array beinhalten: `8`
Geben Sie nun die Elemente der Reihenfolge nach ein:
`83`
`65`
`18`
`25`
`23`
`45`
`45`
`32`

Ergebnis:
2
{% endoutput %}