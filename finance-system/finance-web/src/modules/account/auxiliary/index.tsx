import React, { useState, useRef } from 'react';
import { Card, Tabs, Modal, Form, Input, InputNumber, Button, Space, message, Popconfirm, Alert } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import type { LedgerRecord } from '@/types/account.d';
import { auxiliaryApi } from '@/api/account';

/** 辅助核算管理 */
const AuxiliaryList: React.FC = () => {
  const [activeTab, setActiveTab] = useState<string>('customer');
  const [data, setData] = useState<Record<string, unknown>[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<Record<string, unknown> | null>(null);
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await auxiliaryApi.list(activeTab);
      setData(res.data ?? []);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : '加载辅助核算数据失败');
    } finally { setLoading(false); }
  };

  React.useEffect(() => { loadData(); }, [activeTab]);

  const handleAdd = () => { setEditingRecord(null); form.resetFields(); setModalOpen(true); };
  const handleEdit = (record: Record<string, unknown>) => { setEditingRecord(record); form.setFieldsValue(record); setModalOpen(true); };
  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await auxiliaryApi.update(activeTab, editingRecord.id as number, values);
        message.success('更新成功');
      } else {
        await auxiliaryApi.add(activeTab, values);
        message.success('新增成功');
      }
      setModalOpen(false); loadData();
    } catch { /* validation */ }
  };
  const handleDelete = async (id: number) => { await auxiliaryApi.remove(activeTab, id); message.success('删除成功'); loadData(); };

  // 动态列根据辅助核算类型变化
  const getColumns = (): ProColumns<Record<string, unknown>>[] => {
    if (activeTab === 'customer') {
      return [
        { title: '客户编码', dataIndex: 'customerCode', ellipsis: true },
        { title: '客户名称', dataIndex: 'customerName', ellipsis: true },
        { title: '联系人', dataIndex: 'contact', search: false, ellipsis: true },
        { title: '联系电话', dataIndex: 'phone', search: false, ellipsis: true },
        { title: '地址', dataIndex: 'address', search: false, ellipsis: true },
        { title: '税号', dataIndex: 'taxNo', search: false, ellipsis: true },
        {
          title: '操作', valueType: 'option', width: 140,
          render: (_, record) => (
            <Space>
              <a onClick={() => handleEdit(record)}>编辑</a>
              <Popconfirm title="确认删除?" onConfirm={() => handleDelete(record.id as number)}>
                <a style={{ color: '#ff4d4f' }}>删除</a>
              </Popconfirm>
            </Space>
          ),
        },
      ];
    }
    return [
      { title: '供应商编码', dataIndex: 'supplierCode', ellipsis: true },
      { title: '供应商名称', dataIndex: 'supplierName', ellipsis: true },
      { title: '联系人', dataIndex: 'contact', search: false, ellipsis: true },
      { title: '联系电话', dataIndex: 'phone', search: false, ellipsis: true },
      { title: '地址', dataIndex: 'address', search: false, ellipsis: true },
      { title: '税号', dataIndex: 'taxNo', search: false, ellipsis: true },
      {
        title: '操作', valueType: 'option', width: 140,
        render: (_, record) => (
          <Space>
            <a onClick={() => handleEdit(record)}>编辑</a>
            <Popconfirm title="确认删除?" onConfirm={() => handleDelete(record.id as number)}>
              <a style={{ color: '#ff4d4f' }}>删除</a>
            </Popconfirm>
          </Space>
        ),
      },
    ];
  };

  const getFormFields = () => {
    if (activeTab === 'customer') return (
      <>
        <Form.Item name="customerCode" label="客户编码" rules={[{ required: true }]}><Input /></Form.Item>
        <Form.Item name="customerName" label="客户名称" rules={[{ required: true }]}><Input /></Form.Item>
        <Form.Item name="contact" label="联系人"><Input /></Form.Item>
        <Form.Item name="phone" label="联系电话"><Input /></Form.Item>
        <Form.Item name="address" label="地址"><Input /></Form.Item>
        <Form.Item name="taxNo" label="税号"><Input /></Form.Item>
      </>
    );
    return (
      <>
        <Form.Item name="supplierCode" label="供应商编码" rules={[{ required: true }]}><Input /></Form.Item>
        <Form.Item name="supplierName" label="供应商名称" rules={[{ required: true }]}><Input /></Form.Item>
        <Form.Item name="contact" label="联系人"><Input /></Form.Item>
        <Form.Item name="phone" label="联系电话"><Input /></Form.Item>
        <Form.Item name="address" label="地址"><Input /></Form.Item>
        <Form.Item name="taxNo" label="税号"><Input /></Form.Item>
      </>
    );
  };

  const tabItems = [
    { key: 'customer', label: '客户管理' },
    { key: 'supplier', label: '供应商管理' },
  ];

  return (
    <>
      <Card title="辅助核算">
        {error && <Alert type="error" message={error} showIcon action={<Button size="small" onClick={loadData}>重试</Button>} style={{ marginBottom: 16 }} />}
        <Tabs activeKey={activeTab} onChange={setActiveTab} items={tabItems} />
        <ProTable<Record<string, unknown>>
          headerTitle=""
          rowKey="id"
          columns={getColumns()}
          search={{ labelWidth: 'auto' }}
          dataSource={data}
          loading={loading}
          pagination={false}
          toolBarRender={() => [
            <Button key="add" type="primary" icon={<PlusOutlined />} onClick={handleAdd}>新增</Button>,
            <Button key="refresh" onClick={loadData}>刷新</Button>,
          ]}
        />
      </Card>
      <Modal title={editingRecord ? '编辑' : '新增'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)} width={600}>
        <Form form={form} layout="vertical">{getFormFields()}</Form>
      </Modal>
    </>
  );
};

export default AuxiliaryList;
