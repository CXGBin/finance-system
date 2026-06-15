import React, { useState, useEffect } from 'react';
import { Modal, Form, Input, InputNumber, Tree, Button, message, Popconfirm } from 'antd';
import type { Dept } from '@/types/system.d';
import { deptApi } from '@/api/system';

/** 部门管理页面 */
const DeptList: React.FC = () => {
  const [treeData, setTreeData] = useState<Dept[]>([]);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<Dept | null>(null);
  const [form] = Form.useForm();

  useEffect(() => { loadTree(); }, []);

  const loadTree = async () => {
    const res = await deptApi.tree();
    setTreeData(res.data || []);
  };

  const handleAdd = (parent?: Dept) => {
    setEditingRecord(null);
    form.resetFields();
    if (parent) form.setFieldsValue({ parentId: parent.id });
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
    } catch {}
  };

  const handleDelete = async (id: number) => {
    await deptApi.remove(id);
    message.success('删除成功');
    loadTree();
  };

  const buildTreeNodes = (list: Dept[]): any[] =>
    list.map(item => ({
      key: item.id,
      title: (
        <span>
          {item.deptName}
          <Button type="link" size="small" onClick={(e) => { e.stopPropagation(); handleAdd(item); }}>新增</Button>
          <Button type="link" size="small" onClick={(e) => { e.stopPropagation(); handleEdit(item); }}>编辑</Button>
          <Popconfirm title="确认删除?" onConfirm={() => handleDelete(item.id)}>
            <Button type="link" size="small" danger onClick={(e) => e.stopPropagation()}>删除</Button>
          </Popconfirm>
        </span>
      ),
      children: item.children ? buildTreeNodes(item.children) : [],
    }));

  return (
    <div>
      <div style={{ marginBottom: 16 }}>
        <Button type="primary" onClick={() => handleAdd()}>新增部门</Button>
      </div>
      <Tree showLine defaultExpandAll treeData={buildTreeNodes(treeData)} />
      <Modal title={editingRecord ? '编辑部门' : '新增部门'} open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="parentId" label="上级部门" hidden><Input /></Form.Item>
          <Form.Item name="deptName" label="部门名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="leader" label="负责人"><Input /></Form.Item>
          <Form.Item name="phone" label="联系电话"><Input /></Form.Item>
          <Form.Item name="sortOrder" label="排序" initialValue={0}><InputNumber /></Form.Item>
          <Form.Item name="status" label="状态" valuePropName="checked" initialValue={true}>
            <Input type="number" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default DeptList;
