import React, { useState, useRef } from 'react';
import { Card, Statistic, Row, Col, Table, Tag } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { assetApi } from '@/api/asset';
import type { AssetCard } from '@/types/asset.d';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 资产报表页面 */
const AssetReport: React.FC = () => {
  const actionRef = useRef<ActionType>();

  const columns: ProColumns<AssetCard>[] = [
    { title: '资产编码', dataIndex: 'assetCode', ellipsis: true },
    { title: '资产名称', dataIndex: 'assetName', ellipsis: true },
    { title: '分类', dataIndex: 'categoryName', search: false, ellipsis: true },
    {
      title: '状态', dataIndex: 'status', valueType: 'select',
      valueEnum: { 1: { text: '在用', status: 'Success' }, 2: { text: '停用', status: 'Default' }, 3: { text: '已处置', status: 'Error' } },
    },
    { title: '原值', dataIndex: 'originalValue', align: 'right', search: false, sorter: true, render: (_, r) => <span className="amount-right">{(r.originalValue ?? 0).toFixed(2)}</span> },
    { title: '累计折旧', dataIndex: 'accumulatedDepreciation', align: 'right', search: false, render: (_, r) => <span className="amount-right">{(r.accumulatedDepreciation ?? 0).toFixed(2)}</span> },
    { title: '净值', dataIndex: 'netValue', align: 'right', search: false, render: (_, r) => <span className="amount-right">{((r.originalValue ?? 0) - (r.accumulatedDepreciation ?? 0)).toFixed(2)}</span> },
    { title: '购入日期', dataIndex: 'acquisitionDate', valueType: 'date', sorter: true, search: false, width: 120 },
  ];

  return (
    <Card title="资产台账">
      <ProTable<AssetCard>
        actionRef={actionRef} headerTitle="" rowKey="id" columns={columns}
        search={{ labelWidth: 'auto' }}
        request={createProTableRequest((params) => assetApi.reportLedger(params as any))}
        pagination={{ defaultPageSize: 10, showSizeChanger: true }}
      />
    </Card>
  );
};

export default AssetReport;
