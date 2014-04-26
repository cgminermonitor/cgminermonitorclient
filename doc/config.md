Cgminer Monitor Client Config Documentation
===========================================

Client stores permanent settings in config file. Default config file name is: `CgminerMonitorClient.config`.

You can specify different config file name by passing it as a parameter: `configFile=anotherConfigFile.config`.

Config properties
-----------------
* CgminerPort - port from which the client tries to get data. Default value: 4028.
* WorkerApiKey - ApiKey which identifies your miner. Keep it safe. No default value, 32 random characters (GUID) generated by website.

Example config file
-------------------
```
{
  "CgminerPort": 4028,
  "WorkerApiKey": "12345678901234567890123456789012"
}
```