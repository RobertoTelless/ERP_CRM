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
    
    public partial class FORMULARIO_RESPOSTA_ANEXO
    {
        public int FRAN_CD_ID { get; set; }
        public int FORE_CD_ID { get; set; }
        public string FRAN_NM_TITULO { get; set; }
        public System.DateTime FRAN_DT_ANEXO { get; set; }
        public int FRAN_IN_TIPO { get; set; }
        public string FRAN_AQ_ARQUIVO { get; set; }
        public int FRAN_IN_ATIVO { get; set; }
    
        public virtual FORMULARIO_RESPOSTA FORMULARIO_RESPOSTA { get; set; }
    }
}
