using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateTransfer
{
    public class Pattern
    {
        public int id;
        public string date;
        public string vx_st;
        public string fio;
        public string daterogdenia;
        public string type;
        public string voenkomat;
        public string otpravitel;
        public string ispolnitel;
        public string zaloby;
        public string anamnez;
        public string doi;
        public string rsi;
        public string diagnoz;
        public string isx_st;
        public string datecontroly;
        public string control;
        public string id_user;
        public string godnost;
        public string graf;
        public string vx_diagnoz;
        public string vx_godnost;
        public string protocol_numb;
        public string protocol_numb2;
        public string date_protocol;
        public string date_protocol2;
        public string mail;


        public void PrintFields(string st)
        {
            Console.WriteLine($"{st}");
        }
    }
}
