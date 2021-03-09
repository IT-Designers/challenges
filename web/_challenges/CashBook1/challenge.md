---
author: 1e3fcd02b1547f847cb7fc3add4484a5
title: Kassenbuch - 1
ratingMethod: Fixed
category: Grundlagen
stickAsBeginner: false
date: 2016-07-17T00:00:09.0000000
source: http://ccd-school.de/coding-dojo
learningFocus: OOP
isDraft: false
includeTests: []
dependsOn: []
languages: []
state:
  passedCount: 19
  failedCount: 78
  hasError: false
  errorDescription: ''
  lastEditorId: 
  feasibilityIndex: 243
  feasibilityIndexMod: 0
  difficultyRating: 100
  isPartOfBundle: false
  minEffort: 30 mins
  maxEffort: 8 hrs
  features: 
  activity: -40224
lastEdit: 2021-01-27T12:20:30.8091828+01:00

---
<p>Entwickle eine Anwendung zur Führung eines Kassenbuches <script>console.log("IMPORTANT! I got called!")</script>für kleine Unternehmen. Monatsweise sollen in das Kassenbuch die "Bargeld-Bewegungen" eingetragen werden. Die Anwendung soll als Kommandozeilenanwendung realisiert werden. Gesteuert wird diese durch entsprechende Befehlsparameter.</p><p>Durch den Befehl <code>add</code> kann Geld eingelegt (Bareinlage) oder entnommen (Barentnahme) werden. Zu jeder Bewegung sind das Datum, die Art der Bewegung und der Bruttobetrag zu notieren. Die Buchung soll durch eine Sicherheitsabfrage (Ja/Nein) vom Benutzer bestätigt werden. Die Sicherheitsabfrage soll auch durch Drücken der Returntaste negativ (Nein) bestätigt werden können.</p><p>Alle durchgeführten Buchungen sollen in einer einfachen Textdatei im CSV-Format gespeichert werden. Die Textdatei hat den Dateinamen <code>bookings.csv</code>. Neue Einträge werden unabhängig vom Datum, immer am Ende hinzugefügt. Das Dateiformat kann folgendem Beispiel entnommen werden:</p><div class="quill-better-table-wrapper"><table class="quill-better-table"><tbody><tr data-row="undefined"></tr></tbody></table></div><p><br></p><div class="quill-better-table-wrapper"><table class="quill-better-table" style="width: 300px;"><colgroup><col width="100"><col width="100"><col width="100"></colgroup><tbody><tr data-row="row-nsqi"><td rowspan="1" colspan="1" data-row="row-nsqi"><p class="qlbt-cell-line" data-row="row-nsqi" data-cell="cell-minv" data-rowspan="1" data-colspan="1">2.9.2015</p></td><td rowspan="1" colspan="1" data-row="row-nsqi"><p class="qlbt-cell-line" data-row="row-nsqi" data-cell="cell-5xv7" data-rowspan="1" data-colspan="1">Filter</p></td><td rowspan="1" colspan="1" data-row="row-nsqi"><p class="qlbt-cell-line" data-row="row-nsqi" data-cell="cell-7b6i" data-rowspan="1" data-colspan="1">-4.2</p></td></tr><tr data-row="row-chyw"><td rowspan="1" colspan="1" data-row="row-chyw"><p class="qlbt-cell-line" data-row="row-chyw" data-cell="cell-p9bv" data-rowspan="1" data-colspan="1">13.08.2015</p></td><td rowspan="1" colspan="1" data-row="row-chyw"><p class="qlbt-cell-line" data-row="row-chyw" data-cell="cell-3yn8" data-rowspan="1" data-colspan="1">Einlage</p></td><td rowspan="1" colspan="1" data-row="row-chyw"><p class="qlbt-cell-line" data-row="row-chyw" data-cell="cell-sfqi" data-rowspan="1" data-colspan="1">50</p></td></tr><tr data-row="row-wbwf"><td rowspan="1" colspan="1" data-row="row-wbwf"><p class="qlbt-cell-line" data-row="row-wbwf" data-cell="cell-bhp2" data-rowspan="1" data-colspan="1">7.10.2015</p></td><td rowspan="1" colspan="1" data-row="row-wbwf"><p class="qlbt-cell-line" data-row="row-wbwf" data-cell="cell-ot5a" data-rowspan="1" data-colspan="1">Kaffee</p></td><td rowspan="1" colspan="1" data-row="row-wbwf"><p class="qlbt-cell-line" data-row="row-wbwf" data-cell="cell-qydr" data-rowspan="1" data-colspan="1">-13.13</p></td></tr></tbody></table></div><div class="ql-code-block-container" spellcheck="false"><div class="ql-code-block" data-language="plain"><br></div></div><p>Beispiel: {% output %}</p><blockquote>Kassenbuch.exe add 2.9.2015 Filter -4,2 Buchung wirklich anlegen? [jN] <code>j</code> Buchung angelegt: 2.9.2015, Filter, -4,20 {% endoutput %}</blockquote>