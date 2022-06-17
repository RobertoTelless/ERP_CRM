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
    
    public partial class FILIAL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FILIAL()
        {
            this.CLIENTE = new HashSet<CLIENTE>();
            this.CONTA_RECEBER = new HashSet<CONTA_RECEBER>();
            this.CRM_COMERCIAL = new HashSet<CRM_COMERCIAL>();
            this.CRM_PEDIDO_VENDA = new HashSet<CRM_PEDIDO_VENDA>();
            this.EQUIPAMENTO = new HashSet<EQUIPAMENTO>();
            this.FORNECEDOR = new HashSet<FORNECEDOR>();
            this.MOVIMENTO_ESTOQUE_PRODUTO = new HashSet<MOVIMENTO_ESTOQUE_PRODUTO>();
            this.ORDEM_SERVICO = new HashSet<ORDEM_SERVICO>();
            this.PEDIDO_COMPRA = new HashSet<PEDIDO_COMPRA>();
            this.PEDIDO_VENDA = new HashSet<PEDIDO_VENDA>();
            this.PRECO_PRODUTO = new HashSet<PRECO_PRODUTO>();
            this.PRODUTO_ESTOQUE_FILIAL = new HashSet<PRODUTO_ESTOQUE_FILIAL>();
            this.PRODUTO = new HashSet<PRODUTO>();
            this.PRODUTO_TABELA_PRECO = new HashSet<PRODUTO_TABELA_PRECO>();
            this.SERVICO = new HashSet<SERVICO>();
            this.SERVICO_TABELA_PRECO = new HashSet<SERVICO_TABELA_PRECO>();
            this.TRANSPORTADORA = new HashSet<TRANSPORTADORA>();
            this.USUARIO = new HashSet<USUARIO>();
        }
    
        public int FILI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public string FILI_NM_NOME { get; set; }
        public string FILI_NM_RAZAO { get; set; }
        public string FILI_NR_CNPJ { get; set; }
        public string FILI_NM_EMAIL { get; set; }
        public string FILI_NM_CONTATOS { get; set; }
        public string FILI_NM_TELEFONES { get; set; }
        public string FILI_NM_ENDERECO { get; set; }
        public string FILI_NM_BAIRRO { get; set; }
        public string FILI_NM_CIDADE { get; set; }
        public string FILI_NR_CEP { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public System.DateTime FILI_DT_CADASTRO { get; set; }
        public int FILI_IN_ATIVO { get; set; }
        public string FILI_NR_INSCRICAO_ESTADUAL { get; set; }
        public string FILI_NR_INSCRICAO_MUNICIPAL { get; set; }
        public Nullable<int> FILI_IN_IE_ISENTO { get; set; }
        public string FILI_NR_CNAE { get; set; }
        public string FILI_NM_WEBSITE { get; set; }
        public string FILI_NR_CELULAR { get; set; }
        public string FILI_AQ_LOGOTIPO { get; set; }
        public string FILI_NR_CPF { get; set; }
        public string FILI_NR_RG { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMERCIAL> CRM_COMERCIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EQUIPAMENTO> EQUIPAMENTO { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR> FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORDEM_SERVICO> ORDEM_SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_COMPRA> PEDIDO_COMPRA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_VENDA> PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRECO_PRODUTO> PRECO_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ESTOQUE_FILIAL> PRODUTO_ESTOQUE_FILIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO> PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_TABELA_PRECO> PRODUTO_TABELA_PRECO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SERVICO> SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SERVICO_TABELA_PRECO> SERVICO_TABELA_PRECO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSPORTADORA> TRANSPORTADORA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
    }
}
