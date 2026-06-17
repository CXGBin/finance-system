/** 税种 - 与后端 TaxCategory 对齐 */
export interface TaxCategory {
  id: number;
  taxCode: string; // 后端 TaxCode
  taxName: string; // 后端 TaxName
  taxRate: number; // 后端 TaxRate
  calculationMethod?: number; // 后端 CalculationMethod (int)
  declareCycle?: number; // 后端 DeclareCycle (int)
  subjectId?: number;
  isEnabled: number; // 后端 IsEnabled
  remark?: string;
}

/** 纳税申报 - 与后端 TaxDeclaration 对齐 */
export interface TaxDeclaration {
  id: number;
  taxCategoryId: number; // 后端 TaxCategoryId
  declarePeriod: string; // 后端 DeclarePeriod
  taxAmount: number; // 后端 TaxAmount
  actualPaidAmount: number; // 后端 ActualPaidAmount
  status: number; // 0草稿 1已申报 2已缴纳
  declaredBy?: number; // 后端 DeclaredBy
  remark?: string;
  createTime?: string;
}

/** 发票 - 与后端 TaxInvoice 对齐 */
export interface Invoice {
  id: number;
  invoiceType: number; // 后端 InvoiceType (int)
  invoiceNo: string; // 后端 InvoiceNo
  invoiceDate: string; // 后端 InvoiceDate
  counterpartyName?: string; // 后端 CounterpartyName
  taxAmount: number; // 后端 TaxAmount
  amountWithoutTax: number; // 后端 AmountWithoutTax
  totalAmount: number; // 后端 TotalAmount
  direction: number; // 后端 Direction (int)
  voucherId?: number;
  isVerified: number; // 后端 IsVerified (int: 0未核验 1已核验)
}

/** 税务报表 */
export interface TaxReport {
  taxType: string;
  period: string;
  taxBase: number;
  taxAmount: number;
  paidAmount: number;
  unpaidAmount: number;
}

/** 税务日历项 */
export interface TaxCalendarItem {
  id: number;
  taxType: string;
  period: string;
  deadline: string;
  status: 'pending' | 'submitted' | 'overdue';
}
