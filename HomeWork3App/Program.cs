// Registry - это класс, предоставляющий эксклюзивный доступ к ключам реестра для простых операций.
// RegistryKey - класс реализует методы для просмотра дочерних ключей, создания новых или чтения и модификации существующих,
// включая установку уровней безопасности для них.

using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Security;
using System.Security.AccessControl;
using System.Management;

RegistryKey[] regKeyArray = new RegistryKey[] { Registry.ClassesRoot,
                                                            Registry.CurrentUser,
                                                            Registry.LocalMachine,
                                                            Registry.Users,
                                                            Registry.CurrentConfig,
                                                            Registry.PerformanceData
														  };

foreach (RegistryKey regKey in regKeyArray)
{
    Console.WriteLine("{0} - total elements: {1}.", regKey.Name, regKey.SubKeyCount);
}

// Навигация по реестру
RegistryKey? localMachine = Registry.LocalMachine;
RegistryKey? software = localMachine.OpenSubKey("Software");
RegistryKey? microsoft = software.OpenSubKey("Microsoft");
// software.Close();

Console.WriteLine("{0} - total elements: {1}.", microsoft.Name, microsoft.SubKeyCount);
// microsoft.Close();


// найти профиля всех пользователей
RegistryKey profileList = microsoft.OpenSubKey("Windows NT").OpenSubKey("CurrentVersion").OpenSubKey("ProfileList");

// всего пользователей
Console.WriteLine("{0} - total elements: {1}.", profileList.Name, profileList.SubKeyCount);


List<string> users = new List<string>();
foreach (string keyName in profileList.GetSubKeyNames())
{
    // добавляем пользователя в список users
    RegistryKey key = profileList.OpenSubKey(keyName);
    // S-1-5-21 - пользователи Windows
    if (key is not null && key.Name.Contains("S-1-5-21"))
    {
        users.Add(keyName);
    }
    Console.WriteLine("{0} - all elements: {1}.", key.Name, key.ValueCount);
}

// Находим инфо о пользователях
RegistryKey hkeyUsers = Registry.Users;
List<RegistryKey> usersInfo = new List<RegistryKey>();

foreach (string user in users)
{
    if (hkeyUsers is not null && hkeyUsers.OpenSubKey(user) is RegistryKey)
    {
        usersInfo.Add(hkeyUsers.OpenSubKey(user).OpenSubKey("Volatile Environment"));
    }

}

Console.WriteLine("##############");
Console.WriteLine($"All users: {users.Count}");
users.ForEach(user => { Console.WriteLine(user); });
Console.WriteLine("##############");

foreach (RegistryKey user in usersInfo)
{
    Console.WriteLine($"Пользователь {user}");
    foreach (string valueName in user.GetValueNames())
    {

        Console.WriteLine("{0,-8}: {1}", valueName,
            user.GetValue(valueName).ToString());
    }
    Console.WriteLine("##############");
}

Console.WriteLine("##########################################################");
Console.WriteLine("##################### Users ##############################");
Console.WriteLine("##########################################################");
RegistryKey ke1y = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList");

if (ke1y != null)
{
    string[] subKeyNames = ke1y.GetSubKeyNames();

    foreach (string subKeyName in subKeyNames)
    {
        RegistryKey subKey = ke1y.OpenSubKey(subKeyName);

        if (subKey != null)
        {
            string username = subKey.GetValue("ProfileImagePath")?.ToString();

            if (!string.IsNullOrEmpty(username))
            {
                Console.WriteLine($"Имя пользователя: {username}");
            }

            subKey.Close();
        }
    }

    ke1y.Close();
}
Console.WriteLine("################################################################");
Console.WriteLine("############## Network interfaces ##############################");
Console.WriteLine("################################################################");
NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

foreach (NetworkInterface networkInterface in networkInterfaces)
{
    Console.WriteLine($"Interface name: {networkInterface.Name}");
    Console.WriteLine($"Description: {networkInterface.Description}");
    Console.WriteLine($"Interface type: {networkInterface.NetworkInterfaceType}");
    Console.WriteLine($"Status: {networkInterface.OperationalStatus}");

    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
    foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
    {
        Console.WriteLine($"IP address: {ip.Address}");
        Console.WriteLine($"IP v4 mask: {ip.IPv4Mask}");
    }

    Console.WriteLine();
}

Console.WriteLine("################################################################");
Console.WriteLine("############## Operation system ############################");
Console.WriteLine("################################################################");

ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

ManagementObjectCollection queryResults = searcher.Get();

foreach (ManagementObject os in queryResults)
{
    Console.WriteLine($"Operating System Name: {os["Caption"]}");
    Console.WriteLine($"Version: {os["Version"]}");
    Console.WriteLine($"Build Number: {os["BuildNumber"]}");
    Console.WriteLine($"Manufacturer: {os["Manufacturer"]}");
    Console.WriteLine($"System Directory: {os["SystemDirectory"]}");
    Console.WriteLine($"Windows Directory: {os["WindowsDirectory"]}");
    Console.WriteLine($"Installation Date: {ManagementDateTimeConverter.ToDateTime(os["InstallDate"].ToString())}");
    Console.WriteLine($"Locale: {os["Locale"]}");
    Console.WriteLine($"Country Code: {os["CountryCode"]}");
    Console.WriteLine($"OS Architecture: {os["OSArchitecture"]}");
    Console.WriteLine($"Registered User: {os["RegisteredUser"]}");
    Console.WriteLine($"LastBootUpTime: {os["SerialNumber"]}");
    Console.WriteLine($"LastBootUpTime: {os["LastBootUpTime"]}");
    Console.WriteLine($"TotalVisibleMemorySize: {os["TotalVisibleMemorySize"]}");
    Console.WriteLine($"FreePhysicalMemory: {os["FreePhysicalMemory"]}");
    Console.WriteLine($"TotalVirtualMemorySize: {os["TotalVirtualMemorySize"]}");
    Console.WriteLine($"FreeVirtualMemory: {os["FreeVirtualMemory"]}");
}


Console.WriteLine("################################################################");
Console.WriteLine("###################### Info about devices ######################");
Console.WriteLine("################################################################");

// Create a ManagementObjectSearcher to query for device information
ManagementObjectSearcher searcherDevice = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");

// Execute the query
ManagementObjectCollection queryDeviceResults = searcherDevice.Get();

// Loop through the query results
foreach (ManagementObject device in queryDeviceResults)
{
    // Retrieve and display device properties
    Console.WriteLine($"Device Name: {device["Name"]}");
    Console.WriteLine($"Description: {device["Description"]}");
    Console.WriteLine($"Manufacturer: {device["Manufacturer"]}");
    Console.WriteLine($"Device ID: {device["DeviceID"]}");
    Console.WriteLine($"Status: {device["Status"]}");
    Console.WriteLine($"Status: {device["HardwareID"]}");
    Console.WriteLine($"Status: {device["Service"]}");
    Console.WriteLine($"Status: {device["Caption"]}");
    Console.WriteLine($"Status: {device["ClassGuid"]}");
    Console.WriteLine($"Status: {device["ConfigManagerErrorCode"]}");
    Console.WriteLine();
}

Console.WriteLine("################################################################");
Console.WriteLine("###################### BIOS INFO ###############################");
Console.WriteLine("################################################################");

ManagementObjectSearcher searcherBios = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");

// Execute the query
ManagementObjectCollection queryBiosResults = searcherBios.Get();

// Loop through the query results
foreach (ManagementObject bios in queryBiosResults)
{
    // Retrieve and display BIOS properties
    Console.WriteLine($"Manufacturer: {bios["Manufacturer"]}");
    Console.WriteLine($"Version: {bios["Version"]}");
    Console.WriteLine($"Release Date: {bios["ReleaseDate"]}");
    Console.WriteLine($"BIOS Serial Number: {bios["SerialNumber"]}");
    Console.WriteLine($"SMBIOS Version: {bios["SMBIOSBIOSVersion"]}");
    Console.WriteLine($"BIOS Characteristics: {bios["BIOSCharacteristics"]}");
    Console.WriteLine($"BIOS Characteristics: {bios["Name"]}");
    Console.WriteLine($"BIOS Characteristics: {bios["Description"]}");
    Console.WriteLine($"BIOS Characteristics: {bios["BuildNumber"]}");
    Console.WriteLine($"BIOS Characteristics: {bios["PrimaryBIOS"]}");
    Console.WriteLine($"BIOS Characteristics: {bios["Status"]}");
    Console.WriteLine();
}



Console.WriteLine("################################################################");
Console.WriteLine("########################  PROGRAMS INFO ########################");
Console.WriteLine("################################################################");

RegistryKey keyPrograms = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");

if (keyPrograms != null)
{
    string[] subKeyNames = keyPrograms.GetSubKeyNames();

    foreach (string subKeyName in subKeyNames)
    {
        RegistryKey subKey = keyPrograms.OpenSubKey(subKeyName);

        if (subKey != null)
        {
            string displayName = subKey.GetValue("DisplayName")?.ToString();
            string displayVersion = subKey.GetValue("DisplayVersion")?.ToString();
            string publisher = subKey.GetValue("Publisher")?.ToString();
            string installLocation = subKey.GetValue("InstallLocation")?.ToString();

            Console.WriteLine($"Name: {displayName}");
            Console.WriteLine($"Version: {displayVersion}");
            Console.WriteLine($"Publisher: {publisher}");
            Console.WriteLine($"Install Location: {installLocation}");
            Console.WriteLine();
        }
    }

    keyPrograms.Close();
}


Console.ReadLine();
