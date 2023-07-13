# WpfAppWithSpeechRecognition

This project is a university assignment focused on the usage of speech recognition. It was created using the .NET Framework version 4.8 and requires SQL Server and Microsoft Speech.


## Introduction

The Speech Recognition Usage project is developed as part of a university project. It explores the capabilities of speech recognition technology and demonstrates its usage in simple hotel booking scenario. The project utilizes the .NET Framework version 4.8, SQL Server for data storage, and Microsoft Speech for speech recognition.

## Features

The project includes the following features:

- Speech recognition for Polish language
- Voice commands for controlling the application
- Database integration with SQL Server for storing and retrieving data
- Graphical user interface

## Requirements

To run the Speech Recognition Usage project, ensure that you have the following prerequisites installed on your system:

- .NET Framework 4.8
- SQL Server
- Microsoft Speech Platform SDK (x64) v11.0
- Microsoft Server Speech Text to Speech Voice (pl-PL, Paulina)

## Installation

1. Clone the repository to your local machine using the following command:

   ```
   git clone https://github.com/sosna21/WpfAppWithSpeechRecognition.git
   ```

2. Open the project in Visual Studio or your preferred .NET development environment.

3. Restore the NuGet packages required for the project.

4. Configure the database connection string in the `app.config` file to point to your SQL Server instance.

5. Build the solution to ensure all dependencies are resolved successfully.

## Usage

1. Launch the application.

2. Explore the graphical user interface to navigate through different features.

3. To use speech recognition, ensure that your microphone is connected and functional.

4. Follow the on-screen instructions for voice commands and interact with the application using your voice. You can ask a "Pomoc" ("help" in Polish) command for tips of possible command in each view.

5. You can disable an enable speech recognition and spoken responses by clicing their respective icons located at application bottom left corner.