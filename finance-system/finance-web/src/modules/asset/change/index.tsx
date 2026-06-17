import React from 'react';
import { Card } from 'antd';
import { ProTable, type ProColumns } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { assetApi } from '@/api/asset';

/** 资产变动 */
const AssetChange: React.FC = () => {
  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '资产编号', dataIndex: 'assetCode', search: true, sorter: true },
    { title: '资产名称', dataIndex: 'assetName', search: true },
    { title: '变动类型', dataIndex: 'changeType', valueType: 'select', valueEnum: { 1: { text: '调拨' }, 2: { text: '处置' }, 3: { text: '报废' } }, search: true },
    { title: '变动原因', dataIndex: 'reason', search: true, ellipsis: true },
    { title: '处置收入', dataIndex: 'disposalIncome', align: 'right', sorter: true },
    { title: '操作时间', dataIndex: 'createdTime', sorter: true, search: false },
  ];
  const request = createProTableRequest((params) => assetApi.changeList(params));
  return <Card title="资产变动"><ProTable columns={columns} request={request} search={{ labelWidth: 'auto', defaultCollapsed: true }} rowKey="id" /></Card>;
};

export default AssetChange;
