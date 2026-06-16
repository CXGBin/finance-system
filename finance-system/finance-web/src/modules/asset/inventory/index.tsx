import React from 'react';
import { Card } from 'antd';
import ProTable from '@/components/ProTable';
import { assetApi } from '@/api/asset';
import type { AssetInventory } from '@/types/asset.d';

/** 资产盘点 */
const AssetInventory: React.FC = () => {
  const columns = [
    { title: '资产编号', dataIndex: 'assetCode', key: 'assetCode', search: true },
    { title: '资产名称', dataIndex: 'assetName', key: 'assetName' },
    { title: '存放地点', dataIndex: 'location', key: 'location' },
    { title: '盘点结果', dataIndex: 'result', key: 'result' },
    { title: '盘点人ID', dataIndex: 'operatorId', key: 'operatorId' },
    { title: '盘点时间', dataIndex: 'inventoryTime', key: 'inventoryTime' },
  ];
  return <Card title="资产盘点"><ProTable columns={columns} fetchData={(params) => assetApi.inventoryList(params as any)} /></Card>;
};

export default AssetInventory;
