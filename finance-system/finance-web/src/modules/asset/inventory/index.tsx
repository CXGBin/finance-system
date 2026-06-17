import React from 'react';
import { Card } from 'antd';
import { ProTable, type ProColumns } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { assetApi } from '@/api/asset';

/** 资产盘点 */
const AssetInventory: React.FC = () => {
  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '资产编号', dataIndex: 'assetCode', search: true, sorter: true },
    { title: '资产名称', dataIndex: 'assetName', search: true },
    { title: '存放地点', dataIndex: 'location', search: true },
    { title: '盘点结果', dataIndex: 'result', search: true },
    { title: '盘点时间', dataIndex: 'inventoryTime', sorter: true },
  ];
  const request = createProTableRequest((params) => assetApi.inventoryList(params));
  return <Card title="资产盘点"><ProTable columns={columns} request={request} search={{ labelWidth: 'auto' }} rowKey="id" /></Card>;
};

export default AssetInventory;
