#include <WiFi.h>  
#include <ArduinoHttpClient.h>


// const char* ssid = "Интернет";        
// const char* password = "1357924680"; 

// const char* ssid = "Точка";        
// const char* password = "0123456789"; 

const int ledPin = 2;

const char* ssid = "egoisto";        
const char* password = "truewarrior"; 

// Server settings
const char* serverAddress = ""; // Server IP
int port = 443;                            // Server port

// Device ID
const char* deviceId = "arduino123";

WiFiClient wifiClient;
HttpClient httpClient = HttpClient(wifiClient, serverAddress, port);

// State
bool isMeasuring = false;

void setup() {
  Serial.begin(9600);

  // Connect to Wi-Fi
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  Serial.println("\nConnected to Wi-Fi.");
}

void loop() {
  if (!isMeasuring) {
    Serial.println("Checking for commands...");

    // Send GET request to wait for commands
    String endpoint = "/api/commands/" + String(deviceId);
    httpClient.get(endpoint);

    int statusCode = httpClient.responseStatusCode();
    String response = httpClient.responseBody();

    if (statusCode == 200) {
      Serial.println("Command received: " + response);

      if (response.indexOf("start") >= 0) {
        startSleepMeasurement();
      } else if (response.indexOf("stop") >= 0) {
        stopSleepMeasurement();
      }
    } else {
      Serial.print("Error: ");
      Serial.println(statusCode);
    }

    delay(5000); // Poll interval
  }
}

void startSleepMeasurement() {
  Serial.println("Starting sleep measurement...");
  isMeasuring = true;

  // Add logic to start measurement
}

void stopSleepMeasurement() {
  Serial.println("Stopping sleep measurement...");
  isMeasuring = false;

  // Send collected sleep data to the server
  sendSleepData();
}

void sendSleepData() {
  String endpoint = "/api/sleep-data";
  String sleepData = "{ \"deviceId\": \"" + String(deviceId) + "\", \"data\": \"sleep_quality\" }";

  httpClient.post(endpoint, "application/json", sleepData);

  int statusCode = httpClient.responseStatusCode();
  if (statusCode == 200) {
    Serial.println("Sleep data sent successfully.");
  } else {
    Serial.print("Error sending sleep data: ");
    Serial.println(statusCode);
  }
}