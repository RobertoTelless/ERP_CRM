//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class ATENDIMENTO_CRM
    {
        public int ATCR_CD_ID { get; set; }
        public int ATEN_CD_ID { get; set; }
        public int CRM1_CD_ID { get; set; }
        public string ATCR_IN_ATIVO { get; set; }
    
        public virtual ATENDIMENTO ATENDIMENTO { get; set; }
        public virtual CRM CRM { get; set; }
    }
}
