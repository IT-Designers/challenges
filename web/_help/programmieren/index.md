---
title: Programmieren
---

## Codebeispiel

````
import java.util.Scanner;

public class App {

    public static void main(String[] args) {
        // ...
    }

    //#region Console
    private static Scanner console = new Scanner(System.in);

    private static String readLine() {
        return console.nextLine();
    }

    private static void writeLine(String message) {
        System.out.println(message);
    }

    private static void write(String message) {
        System.out.print(message);
    }
    //#endregion

}
````
