using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ClientBlockChain.Connection
{
    public class OpenPortFirewall
    {

        public static uint OpenPort()
        {
            var listPort = new List<uint> { 5000, 49152, 50000, 10000, 15000, 20000, 8081, 8082, 8090 };

            foreach (uint port in listPort)
            {
                if (!VerifyPortOpen(port)) continue;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    OpenPortWin(port);
                else
                    OpenPortUnix(port);

                return port;
            }

            throw new Exception("No available ports could be opened.");
        }

        public static bool VerifyPortOpen(uint port)
        {
            try
            {
                var tcpListener = new TcpListener(IPAddress.Any, checked((int)port));
                tcpListener.Start();
                tcpListener.Stop();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        private static void OpenPortWin(uint port)
        {
            string ruleName = $"ClientBlockchain_{port}";

            // Remover regra existente (caso exista)
            string deleteCmd = $"advfirewall firewall delete rule name=\"{ruleName}\"";
            string addCmd = $"advfirewall firewall add rule name=\"{ruleName}\" dir=in action=allow protocol=TCP localport={port}";

            try
            {
                ExecuteCommand("netsh", deleteCmd);
                ExecuteCommand("netsh", addCmd);
                Console.WriteLine($"Port {port} opened successfully on Windows Firewall.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening port {port}: {ex.Message}");
            }
        }

        private static void ExecuteCommand(string filename, string arguments)
        {
            var psi = new ProcessStartInfo(filename, arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);
        }

        private static void OpenPortUnix(uint port)
        {
            try
            {
                if (!IsRootUser())
                {
                    Console.WriteLine("Permission denied: Run the application as root or with sudo.");
                    return;
                }

                string firewallCommand = DetectFirewall(port);
                if (string.IsNullOrEmpty(firewallCommand))
                {
                    Console.WriteLine("No compatible firewall detected.");
                    return;
                }

                ExecuteCommand("/bin/bash", $"-c \"{firewallCommand}\"");
                Console.WriteLine($"Port {port} opened successfully on Linux.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening port {port}: {ex.Message}");
            }
        }

        private static bool IsRootUser()
        {
            return Environment.UserName == "root";
        }


        private static string DetectFirewall(uint port)
        {
            try
            {
                var psi = new ProcessStartInfo("/bin/bash", "-c \"which ufw\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new() { StartInfo = psi })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                    {
                        return $"sudo ufw allow {port}/tcp";
                    }
                }

                psi = new ProcessStartInfo("/bin/bash", "-c \"which firewall-cmd\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = psi })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                    {
                        return $"sudo firewall-cmd --add-port={port}/tcp --permanent && sudo firewall-cmd --reload";
                    }
                }

                psi = new ProcessStartInfo("/bin/bash", "-c \"which iptables\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using (var process = new Process { StartInfo = psi })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                    {
                        return $"sudo iptables -A INPUT -p tcp --dport {port} -j ACCEPT";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error when opening the door: {e.Message}");

            }
            return string.Empty;
        }
    }
}