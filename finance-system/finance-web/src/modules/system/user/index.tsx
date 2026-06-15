import React, { useState } from 'react';
import { Modal, Form, Input, Select, Switch, message, Popconfirm } from 'antd';
import type { User } from '@/types/system.d';
import { userApi } from '@/api/system';
import ProTable from '@/components/ProTable';

/** 用户管理页面 */
const UserList: React.FC = () => {
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<User | null>(null);
  const [form] = Form.useForm();

  /** 列定义 */
  const columns = [
    { title: '用户名', dataIndex: 'userName', key: 'userName', search: true },
    { title: '昵称', dataIndex: 'nickName', key: 'nickName' },
    { title: '手机号', dataIndex: 'phone', key: 'phone' },
    { title: '邮箱', dataIndex: 'email', key: 'email' },
    {
      title: '状态', dataIndex: 'status', key: 'status',
      render: (val: number) => (
        <span style={{ color: val === 1 ? '#52c41a' : '#ff4d4f' }}>{val === 1 ? '启用' : '禁用'}</span>
      ),
    },
    { title: '创建时间', dataIndex: 'createdTime', key: 'createdTime', sorter: true },
    {
      title: '操作', key: 'action',
      render: (_: any, record: User) => (
        <a className="table-action" onClick={() => handleEdit(record as User)}>编辑</a>
      ),
    },
  ];

  /** 打开新增弹窗 */
  const handleAdd = () => {
    setEditingRecord(null);
    form.resetFields();
    setModalOpen(true);
  };

  /** 打开编辑弹窗 */
  const handleEdit = (record: User) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  /** 保存（新增/编辑） */
  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await userApi.update({ ...editingRecord, ...values });
        message.success('更新成功');
      } else {
        await userApi.add(values as any);
        message.success('新增成功');
      }
      setModalOpen(false);
    } catch {
      // 表单校验失败
    }
  };

  return (
    <div>
      <ProTable
        columns={columns}
        fetchData={(params) => userApi.list(params as any)}
        toolbarActions={<a onClick={handleAdd}>新增用户</a>}
      />
      <Modal
        title={editingRecord ? '编辑用户' : '新增用户'}
        open={modalOpen}
        onOk={handleSave}
        onCancel={() => setModalOpen(false)}
        width={600}
      >
        <Form form={form} layout="vertical">
          <Form.Item name="userName" label="用户名" rules={[{ required: true, message: '请输入用户名' }]}>
            <Input disabled={!!editingRecord} />
          </Form.Item>
          <Form.Item name="nickName" label="昵称" rules={[{ required: true, message: '请输入昵称' }]}>
            <Input />
          </Form.Item>
          {!editingRecord && (
            <Form.Item name="password" label="密码" rules={[{ required: true, message: '请输入密码' }]}>
              <Input.Password />
            </Form.Item>
          )}
          <Form.Item name="phone" label="手机号">
            <Input />
          </Form.Item>
          <Form.Item name="email" label="邮箱">
            <Input />
          </Form.Item>
          <Form.Item name="deptId" label="部门">
            <Select placeholder="请选择部门" allowClear />
          </Form.Item>
          <Form.Item name="status" label="状态" valuePropName="checked">
            <Switch checkedChildren="启用" unCheckedChildren="禁用" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default UserList;
