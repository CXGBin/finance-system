import React from 'react';
import { Card, Tag, Space, Button } from 'antd';
import ProTable from '@/components/ProTable';
import { taxApi } from '@/api/tax';
import type { TaxDeclaration } from '@/types/tax.d';

/** 纳税申报 */
const TaxDeclaration: React.FC = () => {
  const columns = [
    { title: '申报期间', dataIndex: 'declarePeriod', key: 'declarePeriod', search: true },
    { title: '税种', dataIndex: 'taxName', key: 'taxName' },
    { title: '应纳税额', dataIndex: 'taxAmount', key: 'taxAmount', align: 'right' },
    { title: '实际缴税额', dataIndex: 'actualPaidAmount', key: 'actualPaidAmount', align: 'right' },
    {
      title: '状态', dataIndex: 'status', key: 'status',
      render: (val: number) => [<Tag key={val} color={['', 'default', 'processing', 'success'][val]}>{['', '待申报', '已申报', '已缴纳'][val] || '未知'}</Tag>],
    },
    { title: '操作', key: 'action', render: (_: any, record: TaxDeclaration) => (
      <Space>
        {record.status === 0 && <a onClick={() => taxApi.declarationSubmit(record.id).then(() => window.location.reload())}>申报</a>}
        {record.status === 1 && <a onClick={() => taxApi.declarationSubmit(record.id).then(() => window.location.reload())}>确认缴纳</a>}
      </Space>
    )},
  ];
  return <Card title="纳税申报"><ProTable columns={columns} fetchData={(params) => taxApi.declarationList(params as any)} /></Card>;
};

export default TaxDeclaration;
