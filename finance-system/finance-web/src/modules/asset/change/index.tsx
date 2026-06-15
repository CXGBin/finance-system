import React from 'react';
import { Card } from 'antd';
import ProTable from '@/components/ProTable';
import { assetApi } from '@/api/asset';
import type { AssetChange } from '@/types/asset.d';

/** 资产变动 */
const AssetChange: React.FC = () => {
  const columns = [
    { title: '资产编号', dataIndex: 'assetCode', key: 'assetCode', search: true },
    { title: '资产名称', dataIndex: 'assetName', key: 'assetName' },
    { title: '变动类型', dataIndex: 'changeType', key: 'changeType', render: (val: number) => ['未知', '调拨', '处置', '报废'][val] || '未知' },
    { title: '变动原因', dataIndex: 'reason', key: 'reason', ellipsis: true },
    { title: '处置收入', dataIndex: 'disposalIncome', key: 'disposalIncome', align: 'right' },
    { title: '操作时间', dataIndex: 'createdTime', key: 'createdTime' },
  ];
  return <Card title="资产变动"><ProTable columns={columns} fetchData={(params) => assetApi.changeList(params as any)} /></Card>;
};

export default AssetChange;
