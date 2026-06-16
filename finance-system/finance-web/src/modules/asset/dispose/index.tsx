import React, { useState, useEffect, useRef } from 'react';
import { Card, Button, Modal, Form, InputNumber, Input, Select, DatePicker, message, Tag, Space } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { assetApi } from '@/api/asset';
import type { AssetCard } from '@/types/asset.d';
import { createProTableRequest } from '@/utils/proTableRequest';

const disposeTypes: Record<number, string> = { 4: '处置出售', 5: '报废' };

/** 资产处置页面 */
const AssetDispose: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedAsset, setSelectedAsset] = useState<AssetCard | null>(null);
  const [form] = Form.useForm();

  const columns: ProColumns<AssetCard>[] = [
    { title: '资产编码', dataIndex: 'assetCode', ellipsis: true },
    { title: '资产名称', dataIndex: 'assetName', ellipsis: true },
    { title: '原值', dataIndex: 'originalValue', align: 'right', search: false, sorter: true, render: (_, r) => <span className="amount-right">¥{(r.originalValue ?? 0).toFixed(2)}</span> },
    { title: '累计折旧', dataIndex: 'accumulatedDepreciation', align: 'right', search: false, render: (_, r) => <span className="amount-right">¥{(r.accumulatedDepreciation ?? 0).toFixed(2)}</span> },
    { title: '净值', align: 'right', search: false, render: (_, r) => <span className="amount-right">¥{((r.originalValue ?? 0) - (r.accumulatedDepreciation ?? 0)).toFixed(2)}</span> },
    { title: '状态', dataIndex: 'status', valueType: 'select', search: false, valueEnum: { 1: { text: '在用', status: 'Success' }, 2: { text: '停用', status: 'Default' } } },
    {
      title: '操作', valueType: 'option', width: 80,
      render: (_, record) => (
        <Button size="small" type="link" danger onClick={() => { setSelectedAsset(record); form.resetFields(); setModalOpen(true); }}>处置</Button>
      ),
    },
  ];

  const handleDispose = async () => {
    try {
      const values = await form.validateFields();
      if (!selectedAsset) return;
      const disposeDate = values.disposeDate?.format('YYYY-MM-DD') || new Date().toISOString().split('T')[0];
      await assetApi.dispose(selectedAsset.id, { ...values, disposeDate });
      message.success('资产处置成功');
      setModalOpen(false); actionRef.current?.reload();
    } catch { /* validation */ }
  };

  return (
    <>
      <Card title="资产处置">
        <ProTable<AssetCard>
          actionRef={actionRef} headerTitle="" rowKey="id" columns={columns}
          search={{ labelWidth: 'auto' }}
          request={createProTableRequest((params) => assetApi.cardList({ ...params, status: 1 }))}
          pagination={{ defaultPageSize: 10, showSizeChanger: true }}
        />
      </Card>
      <Modal title={`处置资产 - ${selectedAsset?.assetName || ''}`} open={modalOpen} onOk={handleDispose} onCancel={() => setModalOpen(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="disposeType" label="处置方式" rules={[{ required: true }]}>
            <Select options={Object.entries(disposeTypes).map(([k, v]) => ({ value: Number(k), label: v }))} />
          </Form.Item>
          <Form.Item name="disposalIncome" label="处置收入"><InputNumber min={0} precision={2} style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="reason" label="处置原因" rules={[{ required: true }]}><Input.TextArea rows={3} /></Form.Item>
          <Form.Item name="disposeDate" label="处置日期"><DatePicker style={{ width: '100%' }} /></Form.Item>
        </Form>
        {selectedAsset && (
          <div style={{ background: '#f5f5f5', padding: 12, marginTop: 8, borderRadius: 6 }}>
            <p>原值：¥{selectedAsset.originalValue?.toFixed(2)} | 累计折旧：¥{selectedAsset.accumulatedDepreciation?.toFixed(2)}</p>
            <p>净值：¥{((selectedAsset.originalValue ?? 0) - (selectedAsset.accumulatedDepreciation ?? 0)).toFixed(2)}</p>
          </div>
        )}
      </Modal>
    </>
  );
};

export default AssetDispose;
