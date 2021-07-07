using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface IMaster
    {
        // Segments
        int InsertSegment(OrganizationSchemeTR segmenttb);
        void UpdateSegment(OrganizationSchemeTR segmenttb);
        int DeleteSegment(int segmenttb);

        // Segment Information
        int InsertInfo(OrganizationSchemeInfo segmentinfotb);
        void UpdateInfo(OrganizationSchemeInfo segmentinfotb);
        int DeleteInfo(int segmentinfotb);

        // Segment Type
        void InsertSegmentType(SegmentTypeMaster segmenttypetb);
        void UpdateSegmentType(SegmentTypeMaster segmenttypetb);
        int DeleteSegmentType(int segmenttypetb);

    }
}