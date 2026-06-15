import React, { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Input, InputNumber, message, Popconfirm, Space } from 'antd';
import type { DictType, DictItem } from '@/types/system.d';
import { dictApi } from '@/api/system';

/** 数据字典管理页面 */
const DictList: React.FC = () => {
  const [types, setTypes] = useState<DictType[]>([]);
  const [selectedType, setSelectedType] = useState<number | null>(null);
  const [items, setItems] = useState<DictItem[]>([]);
  const [typeModalOpen, setTypeModalOpen] = useState(false);
  const [itemModalOpen, setItemModalOpen] = useState(false);
  const [editingType, setEditingType] = useState<DictType | null>(null);
  const [editingItem, setEditingItem] = useState<DictItem | null>(null);
  const [typeForm] = Form.useForm();
  const [itemForm] = Form.useForm();

  useEffect(() => { loadTypes(); }, []);

  const loadTypes = async () => {
    const res = await dictApi.typeList();
    setTypes(res.data || []);
  };

  const loadItems = async (dictId: number) => {
    setSelectedType(dictId);
    const res = await dictApi.itemList(dictId);
    setItems(res.data || []);
  };

  const handleAddType = () => {
    setEditingType(null);
    typeForm.resetFields();
    setTypeModalOpen(true);
  };

  const handleEditType = (record: DictType) => {
    setEditingType(record);
    typeForm.setFieldsValue(record);
    setTypeModalOpen(true);
  };

  const handleSaveType = async () => {
    try {
      const values = await typeForm.validateFields();
      if (editingType) {
        await dictApi.typeUpdate({ ...editingType, ...values });
        message.success('更新成功');
      } else {
        await dictApi.typeAdd(values as any);
        message.success('新增成功');
      }
      setTypeModalOpen(false);
      loadTypes();
    } catch {}
  };

  const handleAddItem = () => {
    setEditingItem(null);
    itemForm.resetFields();
    itemForm.setFieldsValue({ dictId: selectedType });
    setItemModalOpen(true);
  };

  const handleEditItem = (record: DictItem) => {
    setEditingItem(record);
    itemForm.setFieldsValue(record);
    setItemModalOpen(true);
  };

  const handleSaveItem = async () => {
    try {
      const values = await itemForm.validateFields();
      if (editingItem) {
        await dictApi.itemUpdate({ ...editingItem, ...values });
        message.success('更新成功');
      } else {
        await dictApi.itemAdd(values as any);
        message.success('新增成功');
      }
      setItemModalOpen(false);
      if (selectedType) loadItems(selectedType);
    } catch {}
  };

  const typeColumns = [
    { title: '字典名称', dataIndex: 'dictName', key: 'dictName' },
    { title: '字典类型', dataIndex: 'dictType', key: 'dictType' },
    { title: '操作', key: 'action', render: (_: any, record: DictType) => (
      <Space>
        <a onClick={() => handleEditType(record as DictType)}>编辑</a>
        <Popconfirm title="确认删除?" onConfirm={() => dictApi.typeRemove(record.id).then(() => { message.success('删除成功'); loadTypes(); })}>
          <a style={{ color: '#ff4d4f' }}>删除</a>
        </Popconfirm>
      </Space>
    )},
  ];

  const itemColumns = [
    { title: '字典标签', dataIndex: 'dictLabel', key: 'dictLabel' },
    { title: '字典键值', dataIndex: 'dictValue', key: 'dictValue' },
    { title: '排序', dataIndex: 'sortOrder', key: 'sortOrder' },
    { title: '操作', key: 'action', render: (_: any, record: DictItem) => (
      <Space>
        <a onClick={() => handleEditItem(record as DictItem)}>编辑</a>
        <Popconfirm title="确认删除?" onConfirm={() => dictApi.itemRemove(record.id).then(() => { message.success('删除成功'); if (selectedType) loadItems(selectedType); })}>
          <a style={{ color: '#ff4d4f' }}>删除</a>
        </Popconfirm>
      </Space>
    )},
  ];

  return (
    <div>
      <Button type="primary" onClick={handleAddType} style={{ marginBottom: 16 }}>新增字典类型</Button>
      <Table
        dataSource={types}
        columns={typeColumns}
        rowKey="id"
        onRow={(record) => ({ onClick: () => loadItems(record.id), style: { cursor: selectedType === record.id ? 'default' : 'pointer', background: selectedType === record.id ? '#e6f7ff' : undefined } })}
      />
      {selectedType && (
        <div style={{ marginTop: 24 }}>
          <div style={{ marginBottom: 12, fontWeight: 'bold' }}>字典项列表 <Button type="link" onClick={handleAddItem}>新增字典项</Button></div>
          <Table dataSource={items} columns={itemColumns} rowKey="id" pagination={false} />
        </div>
      )}
      <Modal title={editingType ? '编辑字典类型' : '新增字典类型'} open={typeModalOpen} onOk={handleSaveType} onCancel={() => setTypeModalOpen(false)}>
        <Form form={typeForm} layout="vertical">
          <Form.Item name="dictName" label="字典名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="dictType" label="字典类型" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="status" label="状态" initialValue={0}><Input type="number" /></Form.Item>
        </Form>
      </Modal>
      <Modal title={editingItem ? '编辑字典项' : '新增字典项'} open={itemModalOpen} onOk={handleSaveItem} onCancel={() => setItemModalOpen(false)}>
        <Form form={itemForm} layout="vertical">
          <Form.Item name="dictId" label="字典ID" hidden><Input /></Form.Item>
          <Form.Item name="dictLabel" label="字典标签" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="dictValue" label="字典键值" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="sortOrder" label="排序" initialValue={0}><InputNumber /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default DictList;
