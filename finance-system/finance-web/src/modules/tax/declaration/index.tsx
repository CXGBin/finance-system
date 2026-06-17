import React, { useRef } from 'react';
import { Card } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { taxApi } from '@/api/tax';

/** 纳税申报 */
const TaxDeclaration: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '申报期间', dataIndex: 'declarePeriod', search: true, sorter: true },
    { title: '税种', dataIndex: 'taxName', search: true },
    { title: '应纳税额', dataIndex: 'taxAmount', align: 'right', sorter: true },
    { title: '实际缴税额', dataIndex: 'actualPaidAmount', align: 'right', sorter: true },
    { title: '状态', dataIndex: 'status', valueType: 'select', valueEnum: { 0: { text: '草稿' }, 1: { text: '待申报' }, 2: { text: '已申报' }, 3: { text: '已缴纳' } }, search: true },
    { title: '创建时间', dataIndex: 'createdTime', sorter: true },
    {
      title: '操作', key: 'action', search: false,
      render: (_, record) => {
        const r = record as any;
        if (r.status === 0) return <a onClick={() => { taxApi.declarationSubmit(r.id); setTimeout(() => actionRef.current?.reload(), 500); }}>申报</a>;
        if (r.status === 1) return <a onClick={() => { taxApi.declarationSubmit(r.id); setTimeout(() => actionRef.current?.reload(), 500); }}>确认缴纳</a>;
        return null;
      },
    },
  ];
  const request = createProTableRequest((params) => taxApi.declarationList(params));
  return <Card title="纳税申报"><ProTable actionRef={actionRef} columns={columns} request={request} search={{ labelWidth: 'auto', defaultCollapsed: true }} rowKey="id" /></Card>;
};

export default TaxDeclaration;
