using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateTransfer
{
    public class Main1
    {
        static void Main()
        {
            MySqlConnection Connection;

            List<Pattern> finalList = new List<Pattern>();
            //List<string> bases = new List<string> {"2014-1", "2014-2", "2015-1", "2015-2", "2016-1", "2016-2", "2017-1", "2017-2", "2018-1", "2018-2", "2019-1", "2019-2",
            //"2020-1", "2020-2", "2021-1", "2021-2", "2022-1", "2022-2", "2023-1", "2023-2" };
            List<string> bases = new List<string> { "2013-1" };

            try
            {
                foreach (string selectedBase in bases)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Console.WriteLine($"============Перенос базы данных: {selectedBase}============");
                    Connection = new MySqlConnection($"server=10.0.0.69;uid=root;database={selectedBase}");
                    Connection.Open();
                    MySqlCommand query = new MySqlCommand();
                    query.Connection = Connection;
                    query.CommandText = "SELECT * FROM protocol";

                    using var Reader = query.ExecuteReader();
                    {
                        while (Reader.Read())
                        {
                            if (string.IsNullOrEmpty(Reader.GetString("fio").Trim()))
                                continue;

                            Pattern CurrentPattern = new Pattern();
                            CurrentPattern.id = Reader.GetInt32("id");
                            CurrentPattern.date = Reader.GetString("date");
                            CurrentPattern.vx_st = Reader.GetString("vx_st");
                            CurrentPattern.fio = Reader.GetString("fio");
                            CurrentPattern.daterogdenia = Reader.GetString("daterogdenia");
                            CurrentPattern.type = Reader.GetString("type");
                            CurrentPattern.voenkomat = Reader.GetString("voenkomat");
                            CurrentPattern.otpravitel = Reader.GetString("otpravitel");
                            CurrentPattern.ispolnitel = Reader.GetString("ispolnitel");
                            CurrentPattern.zaloby = Reader.GetString("zaloby");
                            CurrentPattern.anamnez = Reader.GetString("anamnez");
                            CurrentPattern.doi = Reader.GetString("doi");
                            CurrentPattern.rsi = Reader.GetString("rsi");
                            CurrentPattern.diagnoz = Reader.GetString("diagnoz");
                            CurrentPattern.isx_st = Reader.GetString("isx_st");
                            CurrentPattern.datecontroly = Reader.GetString("datecontroly");
                            CurrentPattern.control = Reader.GetString("control");
                            CurrentPattern.id_user = Reader.GetString("id_user");
                            CurrentPattern.godnost = Reader.GetString("godnost");
                            CurrentPattern.graf = Reader.GetString("graf");
                            CurrentPattern.vx_diagnoz = Reader.GetString("vx_diagnoz");
                            CurrentPattern.vx_godnost = Reader.GetString("vx_godnost");
                            CurrentPattern.protocol_numb = Reader.GetString("protocol_numb");
                            CurrentPattern.protocol_numb2 = Reader.GetString("protocol_numb2");
                            CurrentPattern.date_protocol = Reader.GetString("date_protocol");
                            CurrentPattern.date_protocol2 = Reader.GetString("date_protocol2");
                            CurrentPattern.mail = Reader.GetString("mail");

                            finalList.Add(CurrentPattern);
                            Console.WriteLine($"Отобрана новая запись = {CurrentPattern.fio}");
                        }
                    }
                    Connection.Close();
                    //--------------------------------------Добавление данных в базу 117------------------------------------------------
                    Connection = new MySqlConnection($"server=10.0.0.117;uid=root;database={selectedBase}");
                    Connection.Open();

                    foreach (Pattern pattern in finalList)
                    {
                        string conscriptQuery = "INSERT INTO conscript (creatorID, creationDate, name, birthDate, " +
                            "rvkArticle, rvkDiagnosis, vk, healthCategory, adventPeriod, postPeriod, rvkProtocolDate, " +
                            "rvkProtocolNumber, protocolDate, protocolNumber, letterNumber, inProcess) " +
                            $"VALUES ('{TransferCreatorID(pattern.id_user)}', '{DateOnly.Parse(pattern.date).ToString("dd.MM.yyyy")}', '{pattern.fio.Trim()}', '{pattern.daterogdenia}', " +
                            $"'{pattern.vx_st}', '{pattern.vx_diagnoz.Replace('\'', '"')}', '{TransferVK(pattern.voenkomat)}', '{pattern.vx_godnost}', '{selectedBase}', '', " +
                            $"'{pattern.date_protocol}', '{pattern.protocol_numb}', '{pattern.date_protocol2}', '{pattern.protocol_numb2}', '{pattern.mail}', '0')";
                        MySqlCommand conscriptCommand = new MySqlCommand(conscriptQuery, Connection);

                        string documentQuery = "INSERT INTO documents (conscriptID, article, healthCategory, creatorID, " +
                            "complaint, anamnez, objectData, specialResult, diagnosis, postPeriod, documentDate, " +
                            $"documentType, destinationPoints, reasonForCancel) VALUES (LAST_INSERT_ID(), '{pattern.isx_st}', '{TransferHealthCategory(pattern.godnost + TransferGraf(pattern.graf.Trim()))}'" +
                            $", '{TransferCreatorID(pattern.id_user)}', '{pattern.zaloby.Replace('\'', '"')}', '{pattern.anamnez.Replace('\'', '"')}', '{pattern.doi.Replace('\'', '"')}', '{pattern.rsi.Replace('\'', '"')}', '{pattern.diagnoz.Replace('\'', '"')}'" +
                            $", '{pattern.ispolnitel}', '{DateOnly.Parse(pattern.date).ToString("dd.MM.yyyy")}', '{TransferType(pattern.type)}', '', '{pattern.otpravitel}')";
                        MySqlCommand documentCommand = new MySqlCommand(documentQuery, Connection);

                        conscriptCommand.ExecuteNonQuery();
                        documentCommand.ExecuteNonQuery();

                        Console.WriteLine($"Добавлена новая запись = {pattern.fio.Trim()}");
                    }
                    
                    Connection.Close();
                    finalList.Clear();
                    stopwatch.Stop();
                    Console.WriteLine($"Время затраченное на призыв {selectedBase} = {stopwatch.Elapsed.TotalSeconds}");
                }
            }

            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public static string TransferHealthCategory(string old_godnost) 
        {
            string result = "";
            switch (old_godnost)
            {
                case "А":
                    result = "А1";
                    break;
                case "Б":
                case "Б1":
                case "А4":
                    result = "Б2";
                    break;

                case "В1":
                    result = "В";
                    break;
                case "Г1":
                    result = "В";
                    break;

                default:
                    result = old_godnost;
                    break;
            }
            return result;
        }

        public static string TransferType(string old_type)
        {
            string result = "";
            switch (old_type)
            {
                case "control":
                    result = "control";
                    break;
                case "gal":
                    result = "complaint";
                    break;
                case "voz":
                    result = "return";
                    break;

                default:
                    result = "ERROR";
                    break;
            }
            return result;
        }

        public static string TransferGraf(string old_graf)
        {
            string result = "";
            switch (old_graf)
            {
                case "1":
                case "I":
                    result = "1"; 
                    break;

                case "2":
                case "II":
                    result = "2";
                    break;

                case "3":
                case "III":
                    result = "3"; 
                    break;

                case "4":
                case "IV":
                    result = "4";
                    break;
            }
            return result;
        }

        public static string TransferCreatorID(string old_id)
        {
            string result = "";
            switch (old_id)
            {
                case "1":
                case "13":
                    result = "0";
                    break;

                case "2":
                case "3":
                case "12":
                    result = "9";
                    break;

                case "5":
                    result = "2";
                    break;

                case "6":
                    result = "8";
                    break;

                case "7":
                    result = "3";
                    break;

                case "8":
                    result = "4";
                    break;

                case "9":
                    result = "5";
                    break;

                case "10":
                    result = "6";
                    break;

                case "11":
                    result = "7";
                    break;
                default:
                    result = "ERROR";
                    break;
            }
            return result;
        }

        public static string TransferVK(string old_vk)
        {
            string result = "";
            switch (old_vk)
            {
                case "Центральный округ":
                    result = "41";
                    break;
                case "Чулымский":
                    result = "30";
                    break;
                case "Октябрьский":
                    result = "39";
                    break;
                case "г. Искитим":
                    result = "6";
                    break;
                case "Искитимский":
                    result = "7";
                    break;
                case "Дзержинский":
                    result = "34";
                    break;
                case "Бердский":
                    result = "2";
                    break;
                case "Кыштовский":
                    result = "27";
                    break;
                case "Ленинский":
                    result = "37";
                    break;
                case "Кировский":
                    result = "38";
                    break;
                case "Венгеровский":
                    result = "26";
                    break;
                case "Чановский":
                    result = "25";
                    break;
                case "Калининский":
                    result = "35";
                    break;
                case "Красноозерский":
                    result = "5";
                    break;
                case "Советский":
                    result = "42";
                    break;
                case " Первомайский":
                    result = "43";
                    break;
                case "Тогучинский":
                    result = "23";
                    break;
                case "Новосибирский":
                    result = "31";
                    break;
                case "Коченевский":
                    result = "12";
                    break;
                case "Барабинский":
                    result = "44";
                    break;
                case "Ордынский":
                    result = "18";
                    break;
                case "Каргатский":
                    result = "10";
                    break;
                case "Куйбышевский":
                    result = "14";
                    break;
                case "Мошковский":
                    result = "17";
                    break;
                case "Карасукский":
                    result = "8";
                    break;
                case "Купинский":
                    result = "16";
                    break;
                case "Сузунский":
                    result = "19";
                    break;
                case "Татарский":
                    result = "20";
                    break;
                case "Болотнинский":
                    result = "24";
                    break;
                case "Доволенский":
                    result = "3";
                    break;
                case "Усть-Таркский":
                    result = "21";
                    break;
                case "Маслянинский":
                    result = "28";
                    break;
                case "Черепановский":
                    result = "29";
                    break;
                case "Убинский":
                    result = "11";
                    break;
                case "Чистоозерный":
                    result = "22";
                    break;
                case "Кочковский":
                    result = "4";
                    break;
                case "р.п. Кольцово":
                    result = "32";
                    break;
                case "Здвинский":
                    result = "1";
                    break;
                case "Колыванский":
                    result = "13";
                    break;
                case "Баганский":
                    result = "9";
                    break;
                case "Северный":
                    result = "15";
                    break;
                case "г. Обь":
                    result = "33";
                    break;
                default:
                    result = "ERROR";
                    break;
            }
            return result;
        }
    }
}
