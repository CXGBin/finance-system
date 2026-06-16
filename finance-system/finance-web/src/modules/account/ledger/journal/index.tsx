import React, { useRef } from 'react';
import { Card } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import type { LedgerRecord } from '@/types/account.d';
import { ledgerApi } from '@/api/account';

/** 日记账查询页面（现金/银行日记账） */
const JournalLedger: React.FC = () => {
  const actionRef = useRef<ActionType>();

  const columns: ProColumns<LedgerRecord>[] = [
    { title: '科目编码', dataIndex: 'subjectCode', search: false, width: 120 },
    { title: '科目名称', dataIndex: 'subjectName', search: false, ellipsis: true },
    { title: '凭证号', dataIndex: 'voucherNo', search: false },
    { title: '凭证日期', dataIndex: 'voucherDate', valueType: 'date', search: false, width: 120, sorter: true },
    { title: '摘要', dataIndex: 'summary', search: false, ellipsis: true },
    { title: '借方金额', dataIndex: 'debitAmount', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.debitAmount ?? 0).toFixed(2)}</span> },
    { title: '贷方金额', dataIndex: 'creditAmount', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.creditAmount ?? 0).toFixed(2)}</span> },
  ];

  return (
    <Card title="日记账（现金/银行）">
      <ProTable<LedgerRecord>
        actionRef={actionRef}
        headerTitle=""
        rowKey={(record) => `${record.voucherNo}-${record.subjectId}`}
        columns={columns}
        search={{ labelWidth: 'auto' }}
        request={async (params) => {
          const res = await ledgerApi.journal({
            startPeriod: (params.startPeriod as string) || '',
            endPeriod: (params.endPeriod as string) || '',
          });
          return { data: res.data ?? [], success: true, total: (res.data ?? []).length };
        }}
        pagination={false}
      />
    </Card>
  );
};

export default JournalLedger;
