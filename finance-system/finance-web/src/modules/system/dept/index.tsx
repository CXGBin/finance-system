import React, { useState } from 'react';
import { Modal, Form, Input, InputNumber, Space, Button, message, Popconfirm, Tree, Card, Spin, Alert, Empty } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { Dept } from '@/types/system.d';
import { deptApi } from '@/api/system';

/** 部门管理页面 */
const DeptList: React.FC = () => {
  const [treeData, setTreeData] = useState<Dept[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<Dept | null>(null);
  const [form] = Form.useForm();

  const loadTree = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await deptApi.tree();
      setTreeData(data.data ?? []);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : '加载部门数据失败');
    } finally {
      setLoading(false);
    }
  };

  React.useEffect(() => { loadTree(); }, []);

  const handleAdd = (parent?: Dept) => {
    setEditingRecord(null);
    form.resetFields();
    form.setFieldsValue({ parentId: parent?.id });
    setModalOpen(true);
  };

  const handleEdit = (record: Dept) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await deptApi.update({ ...editingRecord, ...values });
        message.success('更新成功');
      } else {
        await deptApi.add(values as any);
        message.success('新增成功');
      }
      setModalOpen(false);
      loadTree();
    } catch { /* validation */ }
  };

  const handleDelete = async (record: Dept) => {
    await deptApi.remove(record.id);
    message.success('删除成功');
    loadTree();
  };

  const buildTreeNodes = (data: Dept[]) =>
    data.map((item) => ({
      key: item.id,
      title: (
        <span>
          <span style={{ marginRight: 8 }}>{item.deptName}</span>
          <Space size={4}>
            <a onClick={() => handleAdd(item)} style={{ fontSize: 12 }}><PlusOutlined /></a>
            <a onClick={() => handleEdit(item)} style={{ fontSize: 12 }}><EditOutlined /></a>
            <Popconfirm title="确认删除该部门?" onConfirm={() => handleDelete(item)}>
              <a style={{ fontSize: 12, color: '#ff4d4f' }}><DeleteOutlined /></a>
            </Popconfirm>
          </Space>
        </span>
      ),
      children: item.children ? buildTreeNodes(item.children) : undefined,
    }));

  return (
    <>
      <Card title="部门管理" extra={<Button type="primary" icon={<PlusOutlined />} onClick={() => handleAdd()}>新增部门</Button>}>
        <Spin spinning={loading}>
          {error ? (
            <Alert type="error" message={error} showIcon action={<Button size="small" onClick={loadTree}>重试</Button>} />
          ) : treeData.length > 0 ? (
            <Tree showLine defaultExpandAll treeData={buildTreeNodes(treeData)} />
          ) : !loading ? (
            <Empty description="暂无部门数据" />
          ) : null}
        </Spin>
      </Card>
      <Modal title={editingRecord ? '编辑部门' : '新增部门'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)} width={500}>
        <Form form={form} layout="vertical">
          <Form.Item name="parentId" label="上级部门" hidden><InputNumber /></Form.Item>
          <Form.Item name="deptName" label="部门名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="sortOrder" label="排序"><InputNumber style={{ width: '100%' }} /></Form.Item>
          <Form.Item name="leader" label="负责人"><Input /></Form.Item>
          <Form.Item name="phone" label="联系电话"><Input /></Form.Item>
          <Form.Item name="email" label="邮箱"><Input /></Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default DeptList;
