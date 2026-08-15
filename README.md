# AR Storybook – Der Rabe und der Fuchs

Interaktives AR-Storybook auf Basis der Fabel „Der Rabe und der Fuchs“.

Das Projekt verbindet Augmented Reality mit 3D-Charakteranimation und einer zustandsbasierten Story. Die virtuelle Bühne kann auf einer erkannten horizontalen Fläche platziert, verschoben, rotiert und skaliert werden. Nach dem Sperren der Stage startet die Geschichte.

## Voraussetzungen

- Unity 6.5
- Git
- (Git LFS)

## Projekt starten

1. Repository klonen.
2. Falls notwendig:
   ```bash
   git lfs pull
3. Projekt in Unity öffnen
4. SampleScene (Assets/Scenes/SampleScene) öffnen.
5. Play Mode starten.

## Bedienung
- Stage auf einer erkannten horizontalen Fläche platzieren.
- Stage per Drag verschieben.
- Rotation und Skalierung über die Slider anpassen.
- Lock Stage sperrt die Stage und startet die Geschichte.
- Durch Klick auf die Dialogbox wird die Story fortgesetzt.
- Bei der finalen Entscheidung kann der Rabe entweder angetippt oder durch Annäherung beeinflusst werden.
- Unlock Stage erlaubt erneut Änderungen an der Stage.
- Reset Stage entfernt die aktuelle Stage.

## Umgesetzte Funktionen
- AR-Flächenerkennung
- Platzierung einer virtuellen Stage
- AR Anchor
- Verschieben, Rotieren und Skalieren der Stage
- Lock-/Unlock-System
- zustandsbasierter Storyablauf
- zwei AR-Interaktionsmöglichkeiten
- mehrere Raben-Animationen
- zwei unterschiedliche Story-Enden
- Käse-Interaktion

## Mitwirkende
- @JPulzz - AR-Platzierung, Interaktionssystem, Story-Integration, Raben-Animationen und technische Integration
- @Amer21-ta - Rabenmodell

## Assets und externe Ressourcen
- Es wurden keine externen Assets verwendet.

## Verwendete Packages und Abhängigkeiten
- AR Foundation 6.5.0
- ARCore XR Plugin 6.5.0
- ARKit XR Plugin 6.5.0
- XR Simulation Environments 2.1.1
- Universal Render Pipeline 17.5.0
- Input System 1.19.0
- Unity UI (uGUI) 2.5.0

## AI Assistenz
- Generative KI wurde während der Entwicklung unterstützend eingesetzt, insbesondere für technische Erklärungen, Unterstützung bei der Fehlersuche sowie bei der Implementierung einzelner Funktionen.
- Der verwendete Code wurde im Projekt überprüft, angepasst und getestet. Die Funktionsweise der implementierten Komponenten wurde nachvollzogen und bei Bedarf weiterentwickelt.
