using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface IUser
    {
        // Administrator
        void InsertAdmin(UserSecurity admintb);
        void UpdateAdmin(UserSecurity admintb);
        int DeleteAdmin(int admintb, int segment);

        // Employee
        int InsertEmployee(EmployeeMaster employeetb);
        void UpdateEmployee(EmployeeMaster employeetb);
        int DeleteEmployee(int employeetb, int segment);

        // Customer
        int InsertCustomer(CustomerTB customertb);
        void UpdateCustomer(CustomerTB customertb);
        int DeleteCustomer(int customertb, int segment);

        // Supplier
        int InsertSupplier(SupplierTB suppliertb);
        void UpdateSupplier(SupplierTB suppliertb);
        int DeleteSupplier(int suppliertb, int segment);

        // Address Book
        int InsertAddress(AddressBookTB addresstb);
        void UpdateAddress(AddressBookTB addresstb);
        
        
    }
}