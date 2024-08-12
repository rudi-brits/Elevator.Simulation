Otis Simulation Service - Configuration and Testing Guide

Welcome to the Otis Simulation Service project! 
This guide provides an overview of the key components of the project, with a focus on the OtisConfigurationService class, its configuration loading functionality, and how to effectively test it using Moq and NUnit.

Project Overview
The Otis Simulation Service is responsible for simulating elevator operations based on configurations defined in an application settings file. The service handles the loading, validation, and application of these configurations, 
ensuring that the simulation runs with the correct parameters.

Key Components -

  1. OtisConfigurationService
     Purpose: Loads the elevator configuration from an application settings file and validates the configuration to ensure it meets predefined rules.
     Core Method:
     LoadConfiguration(): Reads the JSON configuration, deserializes it, validates it, and applies it to the service.

  2. OtisConfigurationValidator
     Purpose: Validates the configuration data to ensure it adheres to the expected structure and rules.
     Usage: Automatically invoked during the LoadConfiguration method to validate the deserialized configuration.

Configuration Loading Process - 
The configuration loading process involves the following steps:

1. Read Configuration: The LoadConfiguration method reads the JSON configuration from the app settings.
2. Deserialize: The JSON string is deserialized into an OtisConfiguration object using System.Text.Json.
3. Validation: The OtisConfigurationValidator validates the deserialized object to ensure it is correctly structured.
4. Application: If valid, the configuration is applied to the service. If invalid, an exception is thrown.

NOTE: Elevators are allowed to have their own floor configurations within the bounds of the building.

UI Overview - 
The Otis Simulation Service includes a user interface (UI) built using the Terminal.Gui library. 
This UI is a text-based interface designed to simulate elevator operations in a terminal or console environment.

Key Features of the UI - 
  Elevator Display:
  The UI displays the current status of each elevator, including its position, load, and operational status (e.g., moving up, moving down, idle).
  Elevators are represented in a tabular format, providing a clear overview of all active elevators in the simulation.
 
  User Interactions:
  Users can interact with the simulation by making elevator requests through the UI.
  The system processes these requests in real-time, updating the display to reflect elevator movements and status changes.
  
  Dynamic Updates:
  The UI dynamically updates as elevators move between floors, load or unload passengers, and respond to new requests.
  It provides visual feedback on the success or failure of requests, along with any error messages or alerts.

  Console Output:
  The UI uses console output to display logs and messages, helping users track the simulation's progress and any issues that arise during operation.

Getting Started - 

1. Clone the repository.
2. Build the solution in Visual Studio.
3. Run the tests to ensure everything is configured correctly.
4. Modify the configuration and validation logic as needed for your project.
