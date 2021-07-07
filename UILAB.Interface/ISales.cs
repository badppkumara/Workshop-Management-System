using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface ISales
    {
        // Sales Status
        void InsertStatus(SalesStatusTB statustb);
        void UpdateStatus(SalesStatusTB statustb);
        int DeleteStatus(int statustb);

        // Invoices Jobs
        int InsertInvoice(SalesInvoiceTR invoicetb);
        void UpdateInvoice(SalesInvoiceTR invoicetb);
        int DeleteInvoice(int invoicetb);

        // Invoices List
        void InsertInvoiceList(SalesInvoiceListTR invoicelisttb);
        void UpdateInvoiceList(SalesInvoiceListTR invoicetlistb);
        int DeleteInvoiceList(int invoicelisttb);
    }
}