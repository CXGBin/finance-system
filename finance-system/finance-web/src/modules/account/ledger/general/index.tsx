import React, { useRef } from 'react';
import { Card, Space, Select, Button } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { SearchOutlined } from '@ant-design/icons';
import { useState, useEffect } from 'react';
import type { LedgerRecord, Subject } from '@/types/account.d';
import { ledgerApi, subjectApi } from '@/api/account';

/** 总账查询页面 */
const GeneralLedger: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loadingData, setLoadingData] = useState(false);
  const [data, setData] = useState<LedgerRecord[]>([]);

  useEffect(() => {
    subjectApi.tree().then(res => {
      const flat: Subject[] = [];
      const walk = (list: Subject[]) => { list.forEach(s => { flat.push(s); if (s.children) walk(s.children); }); };
      walk(res.data ?? []);
      setSubjects(flat);
    });
  }, []);

  const columns: ProColumns<LedgerRecord>[] = [
    { title: '科目编码', dataIndex: 'subjectCode', search: false, width: 120 },
    { title: '科目名称', dataIndex: 'subjectName', search: false, ellipsis: true },
    { title: '凭证号', dataIndex: 'voucherNo', search: false },
    { title: '凭证日期', dataIndex: 'voucherDate', valueType: 'date', search: false, width: 120, sorter: true },
    { title: '摘要', dataIndex: 'summary', search: false, ellipsis: true },
    { title: '借方金额', dataIndex: 'debitAmount', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.debitAmount ?? 0).toFixed(2)}</span> },
    { title: '贷方金额', dataIndex: 'creditAmount', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.creditAmount ?? 0).toFixed(2)}</span> },
    { title: '余额', dataIndex: 'balance', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.balance ?? 0).toFixed(2)}</span> },
    { title: '方向', dataIndex: 'direction', search: false, width: 60 },
  ];

  return (
    <Card title="总账">
      <ProTable<LedgerRecord>
        actionRef={actionRef}
        headerTitle=""
        rowKey={(record) => `${record.voucherNo}-${record.subjectId}-${record.summary}`}
        columns={columns}
        search={{ labelWidth: 'auto', defaultCollapsed: true }}
        request={async (params) => {
          setLoadingData(true);
          try {
            const res = await ledgerApi.general({
              subjectId: params.subjectId as number,
              startPeriod: (params.startPeriod as string) || '',
              endPeriod: (params.endPeriod as string) || '',
            });
            setData(res.data ?? []);
            return { data: res.data ?? [], success: true, total: (res.data ?? []).length };
          } finally { setLoadingData(false); }
        }}
        columnsState={{ persistenceKey: 'general-ledger-columns' }}
        pagination={false}
      />
    </Card>
  );
};

export default GeneralLedger;
