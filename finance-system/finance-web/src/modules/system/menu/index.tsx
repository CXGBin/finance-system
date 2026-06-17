import React, { useState } from 'react';
import { Modal, Form, Input, InputNumber, Select, Space, Button, message, Popconfirm, Tree, Card, Tag, Spin, Alert, Empty } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { Menu } from '@/types/system.d';
import { menuApi } from '@/api/system';

/** 菜单管理页面 */
const MenuList: React.FC = () => {
  const [treeData, setTreeData] = useState<Menu[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<Menu | null>(null);
  const [form] = Form.useForm();

  const loadTree = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await menuApi.tree();
      setTreeData(data.data ?? []);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : '加载菜单数据失败');
    } finally {
      setLoading(false);
    }
  };

  React.useEffect(() => { loadTree(); }, []);

  const handleAdd = (parent?: Menu) => {
    setEditingRecord(null);
    form.resetFields();
    form.setFieldsValue({ parentId: parent?.id ?? 0, menuType: 2, sortOrder: 0 });
    setModalOpen(true);
  };

  const handleEdit = (record: Menu) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await menuApi.update({ ...editingRecord, ...values });
        message.success('更新成功');
      } else {
        await menuApi.add(values as any);
        message.success('新增成功');
      }
      setModalOpen(false);
      loadTree();
    } catch { /* validation */ }
  };

  const handleDelete = async (record: Menu) => {
    await menuApi.remove(record.id);
    message.success('删除成功');
    loadTree();
  };

  const typeTag = (type: number) => {
    const map: Record<number, { color: string; text: string }> = { 1: { color: 'blue', text: '目录' }, 2: { color: 'green', text: '菜单' }, 3: { color: 'orange', text: '按钮' } };
    const info = map[type] || { color: 'default', text: '未知' };
    return <Tag color={info.color}>{info.text}</Tag>;
  };

  const buildTreeNodes = (data: Menu[]) =>
    data.map((item) => ({
      key: item.id,
      title: (
        <span style={{ display: 'inline-flex', alignItems: 'center', gap: 8 }}>
          {typeTag(item.menuType)}
          <span>{item.menuName}</span>
          {item.path && <span style={{ color: '#999', fontSize: 12 }}>{item.path}</span>}
          <Space size={4}>
            <a onClick={() => handleAdd(item)} style={{ fontSize: 12 }}><PlusOutlined /></a>
            <a onClick={() => handleEdit(item)} style={{ fontSize: 12 }}><EditOutlined /></a>
            <Popconfirm title="确认删除?" onConfirm={() => handleDelete(item)}>
              <a style={{ fontSize: 12, color: '#ff4d4f' }}><DeleteOutlined /></a>
            </Popconfirm>
          </Space>
        </span>
      ),
      children: item.children ? buildTreeNodes(item.children) : undefined,
    }));

  return (
    <>
      <Card title="菜单管理" extra={<Button type="primary" icon={<PlusOutlined />} onClick={() => handleAdd()}>新增菜单</Button>}>
        <Spin spinning={loading}>
          {error ? (
            <Alert type="error" message={error} showIcon action={<Button size="small" onClick={loadTree}>重试</Button>} />
          ) : treeData.length > 0 ? (
            <Tree showLine defaultExpandAll treeData={buildTreeNodes(treeData)} blockNode />
          ) : !loading ? (
            <Empty description="暂无菜单数据" />
          ) : null}
        </Spin>
      </Card>
      <Modal title={editingRecord ? '编辑菜单' : '新增菜单'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)} width={600}>
        <Form form={form} layout="vertical">
          <Form.Item name="parentId" label="上级菜单" hidden><InputNumber /></Form.Item>
          <Form.Item name="menuName" label="菜单名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="menuType" label="类型" rules={[{ required: true }]}>
            <Select options={[{ label: '目录', value: 1 }, { label: '菜单', value: 2 }, { label: '按钮', value: 3 }]} />
          </Form.Item>
          <Form.Item name="path" label="路由路径"><Input /></Form.Item>
          <Form.Item name="component" label="组件路径"><Input /></Form.Item>
          <Form.Item name="icon" label="图标"><Input /></Form.Item>
          <Form.Item name="permission" label="权限标识"><Input /></Form.Item>
          <Form.Item name="sortOrder" label="排序"><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="visible" label="可见" valuePropName="checked">
            <Select options={[{ label: '显示', value: 1 }, { label: '隐藏', value: 0 }]} />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default MenuList;
