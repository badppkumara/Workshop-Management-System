using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface IResource
    {
        // Resources
        int InsertResource(ResourceTR resourcetb);
        void UpdateResource(ResourceTR resourcetb);
        int DeleteResource(int resourcetb);

        // Resource Type
        void InsertType(ResourceType typetb);
        void UpdateType(ResourceType typetb);
        int DeleteType(int typetb);

        // Resource Sub Type
        void InsertSubType(ResourceSubType subtypetb);
        void UpdateSubType(ResourceSubType subtypetb);
        int DeleteSubType(int subtypetb, int user);

        // Resources Assigned
        int InsertResourceUser(ResourceUserTR resourceusertb);
        void UpdateResourceUser(ResourceUserTR resourceusertb);
        int DeleteResourceUser(int resourceusertb);
        int DeleteResourceItem(int resourceusertb, int user);

        // Resource Brand
        int InsertBrand(ResourceBrandTB brandtb);
        void UpdateBrand(ResourceBrandTB brandtb);
        int DeleteBrand(int brandtb);

        // Brand Image
        int InsertBrandImg(FileResourceTR brandimgtb);
        void UpdateBrandImg(FileResourceTR brandimgtb);
        int DeleteBrandImg(int brandimgtb);
    }
}