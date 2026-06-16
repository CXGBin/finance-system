import React, { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, InputNumber, Input, Select, DatePicker, message, Tag } from 'antd';
import { assetApi } from '@/api/asset';

const disposeTypes: Record<number, string> = { 4: '处置出售', 5: '报废' };

export default function AssetDisposePage() {
  const [assets, setAssets] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [selectedAsset, setSelectedAsset] = useState<any>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    assetApi.card.page({ pageIndex: 1, pageSize: 100, status: 1 }).then((res: { list?: AssetCard[] }) => setAssets(res.list || [])).catch(() => {});
  }, []);

  const handleDispose = async () => {
    try {
      const values = await form.validateFields();
      const disposeDate = values.disposeDate?.format('YYYY-MM-DD') || new Date().toISOString().split('T')[0];
      await assetApi.dispose(selectedAsset.id, { ...values, disposeDate });
      message.success('资产处置成功，已生成处置凭证');
      setModalVisible(false);
      form.resetFields();
    } catch {}
  };

  const columns = [
    { title: '资产编号', dataIndex: 'assetCode', width: 120 },
    { title: '资产名称', dataIndex: 'assetName', width: 150 },
    { title: '原值', dataIndex: 'originalValue', width: 120, render: (v: number) => `¥${v.toFixed(2)}` },
    { title: '累计折旧', dataIndex: 'accumulatedDepreciation', width: 120, render: (v: number) => `¥${v.toFixed(2)}` },
    { title: '净值', width: 120, render: (_: unknown, r: AssetCard) => `¥${(r.originalValue - r.accumulatedDepreciation).toFixed(2)}` },
    { title: '状态', dataIndex: 'status', width: 80, render: (v: number) => <Tag color={v === 1 ? 'green' : 'orange'}>{v === 1 ? '使用中' : '闲置'}</Tag> },
    {
      title: '操作', width: 100, fixed: 'right' as const,
      render: (_: unknown, r: AssetCard) => (
        <Button size="small" type="primary" danger onClick={() => { setSelectedAsset(r); setModalVisible(true); form.resetFields(); }}>处置</Button>
      ),
    },
  ];

  return (
    <div>
      <Table columns={columns} dataSource={assets} rowKey="id" loading={loading} scroll={{ x: 1000 }} />
      <Modal title={`处置资产 - ${selectedAsset?.assetName || ''}`} open={modalVisible} onOk={handleDispose} onCancel={() => setModalVisible(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="disposeType" label="处置方式" rules={[{ required: true }]}>
            <Select options={Object.entries(disposeTypes).map(([k, v]) => ({ value: Number(k), label: v }))} />
          </Form.Item>
          <Form.Item name="disposalIncome" label="处置收入">
            <InputNumber min={0} precision={2} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="reason" label="处置原因" rules={[{ required: true }]}>
            <Input.TextArea rows={3} />
          </Form.Item>
          <Form.Item name="disposeDate" label="处置日期">
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
        </Form>
        {selectedAsset && (
          <div style={{ background: '#f5f5f5', padding: 12, marginTop: 8, borderRadius: 4 }}>
            <p>原值：¥{selectedAsset.originalValue?.toFixed(2)} | 累计折旧：¥{selectedAsset.accumulatedDepreciation?.toFixed(2)}</p>
            <p>净值：¥{(selectedAsset.originalValue - selectedAsset.accumulatedDepreciation).toFixed(2)}</p>
          </div>
        )}
      </Modal>
    </div>
  );
}
