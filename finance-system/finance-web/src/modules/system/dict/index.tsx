import React, { useState, useRef } from 'react';
import { Modal, Form, Input, InputNumber, Switch, message, Popconfirm, Space, Button, Tag, Card } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { PlusOutlined } from '@ant-design/icons';
import type { DictType, DictItem } from '@/types/system.d';
import { dictApi } from '@/api/system';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 数据字典管理页面 */
const DictList: React.FC = () => {
  const typeActionRef = useRef<ActionType>();
  const [typeModalOpen, setTypeModalOpen] = useState(false);
  const [editingType, setEditingType] = useState<DictType | null>(null);
  const [typeForm] = Form.useForm();

  // 字典项相关
  const [selectedType, setSelectedType] = useState<string>('');
  const [items, setItems] = useState<DictItem[]>([]);
  const [itemModalOpen, setItemModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<DictItem | null>(null);
  const [itemForm] = Form.useForm();

  const typeColumns: ProColumns<DictType>[] = [
    { title: '字典名称', dataIndex: 'dictName', ellipsis: true },
    { title: '字典类型', dataIndex: 'dictType', ellipsis: true },
    {
      title: '状态', dataIndex: 'status', valueType: 'select',
      valueEnum: { 1: { text: '启用', status: 'Success' }, 0: { text: '停用', status: 'Default' } },
    },
    { title: '备注', dataIndex: 'remark', search: false, ellipsis: true },
    {
      title: '操作', valueType: 'option', width: 180,
      render: (_, record) => (
        <Space>
          <a onClick={() => loadItems(record.dictType)}>字典项</a>
          <a onClick={() => handleEditType(record)}>编辑</a>
          <Popconfirm title="确认删除?" onConfirm={() => handleDeleteType(record)}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  const handleAddType = () => { setEditingType(null); typeForm.resetFields(); typeForm.setFieldsValue({ status: 1 }); setTypeModalOpen(true); };
  const handleEditType = (record: DictType) => { setEditingType(record); typeForm.setFieldsValue(record); setTypeModalOpen(true); };
  const handleSaveType = async () => {
    try {
      const values = await typeForm.validateFields();
      if (editingType) { await dictApi.typeUpdate({ ...editingType, ...values }); message.success('更新成功'); }
      else { await dictApi.typeAdd(values as any); message.success('新增成功'); }
      setTypeModalOpen(false); typeActionRef.current?.reload();
    } catch { /* validation */ }
  };
  const handleDeleteType = async (record: DictType) => { await dictApi.typeRemove(record.id); message.success('删除成功'); typeActionRef.current?.reload(); };

  const loadItems = async (dictType: string) => {
    setSelectedType(dictType);
    const data = await dictApi.itemList(dictType);
    setItems(data.data ?? []);
  };

  const handleAddItem = () => { setEditingItem(null); itemForm.resetFields(); itemForm.setFieldsValue({ dictType: selectedType, sortOrder: 0, status: 1 }); setItemModalOpen(true); };
  const handleEditItem = (record: DictItem) => { setEditingItem(record); itemForm.setFieldsValue(record); setItemModalOpen(true); };
  const handleSaveItem = async () => {
    try {
      const values = await itemForm.validateFields();
      if (editingItem) { await dictApi.itemUpdate({ ...editingItem, ...values }); message.success('更新成功'); }
      else { await dictApi.itemAdd(values as any); message.success('新增成功'); }
      setItemModalOpen(false); loadItems(selectedType);
    } catch { /* validation */ }
  };
  const handleDeleteItem = async (record: DictItem) => { await dictApi.itemRemove(record.id); message.success('删除成功'); loadItems(selectedType); };

  const itemColumns: ProColumns<DictItem>[] = [
    { title: '字典标签', dataIndex: 'dictLabel', ellipsis: true },
    { title: '字典值', dataIndex: 'dictValue', ellipsis: true },
    { title: '排序', dataIndex: 'sortOrder', sorter: true, search: false, width: 80, align: 'right' },
    {
      title: '状态', dataIndex: 'status', valueType: 'select', search: false,
      valueEnum: { 1: { text: '启用', status: 'Success' }, 0: { text: '停用', status: 'Default' } },
    },
    { title: '备注', dataIndex: 'remark', search: false, ellipsis: true },
    {
      title: '操作', valueType: 'option', width: 140,
      render: (_, record) => (
        <Space>
          <a onClick={() => handleEditItem(record)}>编辑</a>
          <Popconfirm title="确认删除?" onConfirm={() => handleDeleteItem(record)}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <>
      <ProTable<DictType>
        actionRef={typeActionRef} headerTitle="字典类型" rowKey="id" columns={typeColumns}
        search={{ labelWidth: 'auto' }}
        request={createProTableRequest((params) => dictApi.typePage(params as any))}
        toolBarRender={() => [<Button key="add" type="primary" icon={<PlusOutlined />} onClick={handleAddType}>新增类型</Button>]}
        pagination={{ defaultPageSize: 10, showSizeChanger: true }}
      />
      {selectedType && (
        <Card title={`字典项 - ${selectedType}`} style={{ marginTop: 16 }} extra={<Button type="primary" size="small" onClick={handleAddItem}>新增字典项</Button>}>
          <ProTable<DictItem>
            headerTitle="" rowKey="id" columns={itemColumns} search={false}
            dataSource={items} pagination={false}
            toolBarRender={false}
          />
        </Card>
      )}
      <Modal title={editingType ? '编辑字典类型' : '新增字典类型'} open={typeModalOpen} onOk={handleSaveType} onCancel={() => setTypeModalOpen(false)} width={500}>
        <Form form={typeForm} layout="vertical">
          <Form.Item name="dictName" label="字典名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="dictType" label="字典类型" rules={[{ required: true }]}><Input disabled={!!editingType} /></Form.Item>
          <Form.Item name="status" label="状态" valuePropName="checked"><Switch checkedChildren="启用" unCheckedChildren="禁用" /></Form.Item>
          <Form.Item name="remark" label="备注"><Input.TextArea rows={2} /></Form.Item>
        </Form>
      </Modal>
      <Modal title={editingItem ? '编辑字典项' : '新增字典项'} open={itemModalOpen} onOk={handleSaveItem} onCancel={() => setItemModalOpen(false)} width={500}>
        <Form form={itemForm} layout="vertical">
          <Form.Item name="dictType" label="字典类型" hidden><Input /></Form.Item>
          <Form.Item name="dictLabel" label="字典标签" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="dictValue" label="字典值" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="sortOrder" label="排序"><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="status" label="状态" valuePropName="checked"><Switch checkedChildren="启用" unCheckedChildren="禁用" /></Form.Item>
          <Form.Item name="remark" label="备注"><Input.TextArea rows={2} /></Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default DictList;
