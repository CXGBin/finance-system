import React, { useState, useRef } from 'react';
import { Modal, Form, Input, Select, Switch, message, Popconfirm, Space, Button, Tag } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import type { User } from '@/types/system.d';
import { userApi } from '@/api/system';
import { createProTableRequest } from '@/utils/proTableRequest';

/** 用户管理页面 */
const UserList: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<User | null>(null);
  const [form] = Form.useForm();

  const columns: ProColumns<User>[] = [
    {
      title: '用户名',
      dataIndex: 'username',
      ellipsis: true,
    },
    {
      title: '真实姓名',
      dataIndex: 'realName',
      ellipsis: true,
    },
    {
      title: '手机号',
      dataIndex: 'phone',
      ellipsis: true,
    },
    {
      title: '邮箱',
      dataIndex: 'email',
      ellipsis: true,
      search: false,
    },
    {
      title: '部门',
      dataIndex: 'deptName',
      search: false,
    },
    {
      title: '状态',
      dataIndex: 'status',
      valueType: 'select',
      valueEnum: {
        1: { text: '启用', status: 'Success' },
        0: { text: '停用', status: 'Default' },
      },
      render: (_, record) => (
        <Switch
          checked={record.status === 1}
          checkedChildren="启用"
          unCheckedChildren="禁用"
          size="small"
          onChange={() => handleToggleStatus(record)}
        />
      ),
    },
    {
      title: '创建时间',
      dataIndex: 'createdTime',
      valueType: 'dateTime',
      sorter: true,
      search: false,
      width: 180,
    },
    {
      title: '操作',
      valueType: 'option',
      width: 200,
      render: (_, record) => (
        <Space>
          <a onClick={() => handleEdit(record)}>编辑</a>
          <a onClick={() => handleResetPassword(record)}>重置密码</a>
          <Popconfirm title="确认删除该用户?" onConfirm={() => handleDelete(record)}>
            <a style={{ color: '#ff4d4f' }}>删除</a>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  const handleAdd = () => {
    setEditingRecord(null);
    form.resetFields();
    setModalOpen(true);
  };

  const handleEdit = (record: User) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

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
      actionRef.current?.reload();
    } catch {
      // 表单校验失败
    }
  };

  const handleToggleStatus = async (record: User) => {
    const newStatus = record.status === 1 ? 0 : 1;
    await userApi.toggleStatus(record.id, newStatus);
    message.success(newStatus === 1 ? '已启用' : '已禁用');
    actionRef.current?.reload();
  };

  const handleResetPassword = async (record: User) => {
    await userApi.resetPassword(record.id);
    message.success('密码已重置');
  };

  const handleDelete = async (record: User) => {
    await userApi.remove(record.id);
    message.success('删除成功');
    actionRef.current?.reload();
  };

  return (
    <>
      <ProTable<User>
        actionRef={actionRef}
        headerTitle="用户管理"
        rowKey="id"
        columns={columns}
        search={{ labelWidth: 'auto' }}
        request={createProTableRequest((params) => userApi.list(params as any))}
        toolBarRender={() => [
          <Button key="add" type="primary" onClick={handleAdd}>新增用户</Button>,
        ]}
        pagination={{ defaultPageSize: 10, showSizeChanger: true }}
      />
      <Modal
        title={editingRecord ? '编辑用户' : '新增用户'}
        open={modalOpen}
        onOk={handleSave}
        onCancel={() => setModalOpen(false)}
        width={600}
      >
        <Form form={form} layout="vertical">
          <Form.Item name="username" label="用户名" rules={[{ required: true, message: '请输入用户名' }]}>
            <Input disabled={!!editingRecord} />
          </Form.Item>
          <Form.Item name="realName" label="真实姓名" rules={[{ required: true, message: '请输入真实姓名' }]}>
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
    </>
  );
};

export default UserList;
