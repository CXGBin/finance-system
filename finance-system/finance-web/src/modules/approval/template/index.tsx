import React, { useState, useRef } from 'react';
import { Modal, Form, Input } from 'antd';
import { ProTable, type ProColumns, type ActionType } from '@ant-design/pro-components';
import { createProTableRequest } from '@/utils/proTableRequest';
import { approvalApi } from '@/api/approval';
import { message } from 'antd';

/** 审批模板管理 */
const ApprovalTemplate: React.FC = () => {
  const [modalOpen, setModalOpen] = useState(false);
  const [form] = Form.useForm();
  const actionRef = useRef<ActionType>();

  const columns: ProColumns<Record<string, unknown>>[] = [
    { title: '模板名称', dataIndex: 'flowName', search: true, sorter: true },
    { title: '流程编码', dataIndex: 'flowCode', search: true },
    { title: '模块类型', dataIndex: 'moduleType', search: true },
    { title: '状态', dataIndex: 'status', valueType: 'select', valueEnum: { 0: { text: '禁用' }, 1: { text: '启用' } }, search: true },
    { title: '创建时间', dataIndex: 'createdTime', sorter: true },
  ];

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      await approvalApi.templateAdd(values as any);
      message.success('保存成功');
      setModalOpen(false);
      actionRef.current?.reload();
    } catch {}
  };

  const request = createProTableRequest((params) => approvalApi.templateList(params));

  return (
    <div>
      <ProTable
        actionRef={actionRef}
        columns={columns}
        request={request}
        search={{ labelWidth: 'auto', defaultCollapsed: true }}
        rowKey="id"
        toolBarRender={() => [<a key="add" onClick={() => { form.resetFields(); setModalOpen(true); }}>新增模板</a>]}
      />
      <Modal title="新增审批模板" open={modalOpen} onOk={handleSave} onCancel={() => setModalOpen(false)}>
        <Form form={form} layout="vertical">
          <Form.Item name="flowName" label="模板名称" rules={[{ required: true }]}><Input /></Form.Item>
          <Form.Item name="flowCode" label="流程编码"><Input /></Form.Item>
          <Form.Item name="moduleType" label="模块类型"><Input /></Form.Item>
          <Form.Item name="description" label="描述"><Input.TextArea /></Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default ApprovalTemplate;
