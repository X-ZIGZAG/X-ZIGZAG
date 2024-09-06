
<p align="center">
  <img src="logo.gif"><br>
</p>


**X-ZIGZAG** is a **lightweight** and **stealthy** Windows Remote Access Trojan (RAT) designed for **educational purposes**. With a focus on **small size** and **undetectability**, X-ZIGZAG operates entirely in RAM, ensuring no traces are left on the target system. This tool is built without relying on any external libraries or third-party dependencies, making it both efficient and versatile.

---
## 🚨 Disclaimer
> **This project is for educational purposes only.** Unauthorized use on any system without the owner’s explicit consent is illegal and unethical. The creator assumes no responsibility for any misuse or damage caused by this software.


## 🌟 Key Features

- **💣 Self Destruct:** Completely erase itself from the system without leaving any trace.
- **⬇️ Download:** Fetch and execute files from a remote server.
- **📶 WiFi Passwords:** Retrieve stored WiFi passwords effortlessly.
- **🔐 Chromium Browsers Data:** Extract saved passwords, credit card details, and cookies from Chromium-based browsers.
- **🖥️ System Info:** Gather comprehensive system information.
- **📸 Screenshots:** Capture screenshots of the target machine in real-time.
- **📤 Upload:** Seamlessly send files from the target system to your server.
- **🛡️ VPN/Proxy Detection:** Detect if the user is accessing the endpoints via a VPN or proxy. If detected, the RAT will shut down immediately.
- **🚫 BlackList IPs:** Automatically avoid communication with IP addresses from known data centers (e.g., Google, Amazon, Azure, OVH). If the RAT detects that it is running from one of these IPs, it will shut down without performing any actions.
- **👻 Hide:** Operate in stealth mode to avoid detection.
- **♻️ AutoStart Setup:** Establish persistence on the target machine for continuous operation.
- **🛑 VM/Server/RDP/VPS Detection:** Prevent execution in virtualized environments, servers, or remote desktop sessions.
- **🖥️ CMD / PowerShell Execution:** Execute custom commands via CMD or PowerShell.
- **🔧 Execute C# or VB.NET Code:** Run custom C# or VB.NET code dynamically on the target system.

## ⚙️ How It Works

- **🔗 Communication:** X-ZIGZAG communicates with a predefined server endpoint at intervals specified by the creator. It retrieves and executes instructions, returning results to the server for later analysis if necessary.

- **🧠 In-RAM Operation:** All operations are executed in RAM, ensuring that no files are written to the disk, significantly reducing the risk of detection.

## ⚖️ Legal & Ethical Considerations
The use of X-ZIGZAG on any system without explicit permission from the system’s owner is illegal. This tool is intended purely for educational purposes, allowing security professionals to study and understand the tactics, techniques, and procedures (TTPs) employed by malicious actors.

## 🛠️ Technologies Used

- **Client (Target Machine):** .NET Framework 4.6.1 - Windows Forms Application
- **Server Side:** ASP.NET 8 Web API, Entity Framework, PostgreSQL, Angular 18

## 🚀 Installation

For a comprehensive installation guide, please refer to the release section of our GitHub repository: [X-ZIGZAG Releases](releases).

Get started with ease by following the detailed instructions provided there!

## 📃 To-Do List
- [ ] 👻 Improve Undetectability
- [ ] ⚡ Optimize Size
- [ ] 📸 Webcam (Not stable and too risky)
- [ ] 📸 Webcam (Not stable and too risky)
- [ ] 📝 Keylogger (Doesn't support all keyboard layouts)
- [ ] 📄 Firefox Browser Data
- [ ] 🔴 Live Interaction (Using sockets)

## 🔧 Contribution

Contributions are welcome! Please fork this repository, create a feature branch, and submit a pull request.

## ©️ License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

**🛡️ Stay ethical, stay safe.**
