---
author: 1e3fcd02b1547f847cb7fc3add4484a5
title: Pfad zerlegen
ratingMethod: Fixed
category: Grundlagen
stickAsBeginner: false
date: 2019-08-07T15:41:56.7193722+02:00
source: none
learningFocus: 
isDraft: false
includeTests: []
dependsOn: []
languages: []
state:
  passedCount: 21
  failedCount: 39
  hasError: false
  errorDescription: ''
  lastEditorId: 
  feasibilityIndex: 538
  feasibilityIndexMod: 0
  difficultyRating: 80
  isPartOfBundle: false
  minEffort: 30 mins
  maxEffort: 8 hrs
  features:
  - String Operations
  activity: -13440
lastEdit: 2019-08-07T15:41:56.7193722+02:00

---
Erstelle eine kleine Konsolenanwendung, welcher beim Programmstart einen Dateipfad als Parameter übergeben wird. Dieser Pfad soll dann in seine Bestandteile Verzeichnis, Name und Erweiterung zerlegt werden. Die Bestandteile sollen dann auf dem Bildschirm ausgegeben werden.

Beispiel:
{% output %}
> PathSplitting.exe
Dateipfad: `c:\Program Files (x86)\Microsoft Office\Office15\Word.exe`

Verzeichnis: c:\Program Files (x86)\Microsoft Office\Office15
Dateiname:   Word
Erweiterung: exe
{% endoutput %}