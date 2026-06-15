import React, { useState } from 'react';
import { Modal, Form, Input, InputNumber, Select, Button, message, Popconfirm, Space, Table } from 'antd';
import { taxApi } from '@/api/tax';
import type { TaxType } from '@/types/tax.d';

/** 税种管理 */
const TaxTypeList: React.FC = () => {
  const [data, setData] = useState<TaxType[]>([]);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<TaxType | null>(null);
  const [form] = Form.useForm();

  const loadData = async () => { const res = await taxApi.typeList(); setData(res.data || []); };
  React.useEffect(() => { loadData(); }, []);

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) { await taxApi.typeUpdate({ ...editingRecord, ...values }); message.success('更新成功'); }
      else { await taxApi.typeAdd(values as any); message.success('新增成功'); }
      setModalOpen(false); loadData();
    } catch {}
  };

  const columns = [
    { title: '编码', dataIndex: 'taxCode', key: 'taxCode' },
    { title: '名称', dataIndex: 'taxName', key: 'taxName' },
    { title: '税率(%)', dataIndex: 'taxRate', key: 'taxRate', align: 'right' },
    { title: '申报周期', dataIndex: 'declareCycle', key: 'declareCycle', render: (v: number) => ['', '月', '季', '年'][v] || '未知' },
    { title: '操作', key: 'action', render: (_: unknown, r: TaxType) => (
      <Space>
        <a onClick={() => { setEditingRecord(r); form.setFieldsValue(r); setModalOpen(true); }}>编辑</a>
        <Popconfirm title="确认删除?" onConfirm={() => taxApi.typeRemove(r.id).then(() => { message.success('删除成功'); loadData(); })}><a style={{ color: '#ff4d4f' }}>删除</a></Popconfirm>
      </Space>
    )},
  ];

  return (
    <div>
      <Button type="primary" onClick={() => { setEditingRecord(null); form.resetFields(); setModalOpen(true); }} style={{ marginBottom: 16 }}>新增税种</Button>
      <Table columns={columns} dataSource={data} rowKey="id" />
      <Modal title={editingRecord ? '编辑' : '新增'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="taxCode" label="编码" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="taxName" label="名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="taxRate" label="税率(%)" rules={[{ required: true }]}><InputNumber style={{ width: '100%' }} step={0.01} /></Form.Item>
          <Form.Item name="declareCycle" label="申报周期"><Select options={[{ label: '月', value: 1 }, { label: '季', value: 2 }, { label: '年', value: 3 }]} /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default TaxTypeList;
