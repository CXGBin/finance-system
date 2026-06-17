import React, { useState, useRef } from "react";
import {
  Modal,
  Form,
  Input,
  InputNumber,
  Select,
  Switch,
  Space,
  Button,
  message,
  Popconfirm,
  Tree,
  Card,
  Tag,
  Dropdown,
  Spin,
  Alert,
  Empty,
} from "antd";
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  ExportOutlined,
  ImportOutlined,
} from "@ant-design/icons";
import type { Subject } from "@/types/account.d";
import { subjectApi, subjectImportExportApi } from "@/api/account";

/** 科目管理页面 */
const SubjectList: React.FC = () => {
  const [treeData, setTreeData] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<Subject | null>(null);
  const [form] = Form.useForm();

  const loadTree = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await subjectApi.tree();
      setTreeData(data.data ?? []);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "加载科目数据失败");
    } finally {
      setLoading(false);
    }
  };

  React.useEffect(() => {
    loadTree();
  }, []);

  const handleAdd = (parent?: Subject) => {
    setEditingRecord(null);
    form.resetFields();
    form.setFieldsValue({
      parentId: parent?.id ?? 0,
      isEnabled: 1,
      balanceDirection: 1,
    });
    setModalOpen(true);
  };

  const handleEdit = (record: Subject) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      if (editingRecord) {
        await subjectApi.update({ ...editingRecord, ...values });
        message.success("更新成功");
      } else {
        await subjectApi.add(values as any);
        message.success("新增成功");
      }
      setModalOpen(false);
      loadTree();
    } catch {
      /* validation */
    }
  };

  const handleDelete = async (record: Subject) => {
    await subjectApi.remove(record.id);
    message.success("删除成功");
    loadTree();
  };

  const typeTag = (type: number) => {
    const map: Record<number, { color: string; text: string }> = {
      1: { color: "blue", text: "资产" },
      2: { color: "green", text: "负债" },
      3: { color: "orange", text: "权益" },
      4: { color: "purple", text: "成本" },
      5: { color: "red", text: "损益" },
    };
    const info = map[type] || { color: "default", text: "其他" };
    return <Tag color={info.color}>{info.text}</Tag>;
  };

  const buildTreeNodes = (data: Subject[]) =>
    data.map((item) => ({
      key: item.id,
      title: (
        <span
          style={{
            display: "inline-flex",
            alignItems: "center",
            gap: 6,
            flexWrap: "wrap",
          }}
        >
          <span style={{ fontFamily: "monospace" }}>{item.subjectCode}</span>
          <span>{item.subjectName}</span>
          {typeTag(item.subjectType)}
          {item.isEnabled === 1 ? (
            <Tag color="success" style={{ fontSize: 10 }}>
              启用
            </Tag>
          ) : (
            <Tag color="default" style={{ fontSize: 10 }}>
              停用
            </Tag>
          )}
          {item.isCash === 1 && (
            <Tag color="cyan" style={{ fontSize: 10 }}>
              现金
            </Tag>
          )}
          {item.isBank === 1 && (
            <Tag color="blue" style={{ fontSize: 10 }}>
              银行
            </Tag>
          )}
          <Space size={4}>
            <a onClick={() => handleAdd(item)} style={{ fontSize: 12 }}>
              <PlusOutlined />
            </a>
            <a onClick={() => handleEdit(item)} style={{ fontSize: 12 }}>
              <EditOutlined />
            </a>
            <Popconfirm
              title="确认删除该科目?"
              onConfirm={() => handleDelete(item)}
            >
              <a style={{ fontSize: 12, color: "#ff4d4f" }}>
                <DeleteOutlined />
              </a>
            </Popconfirm>
          </Space>
        </span>
      ),
      children: item.children ? buildTreeNodes(item.children) : undefined,
    }));

  return (
    <>
      <Card
        title="科目管理"
        extra={
          <Space>
            <Button
              icon={<ExportOutlined />}
              onClick={() => subjectImportExportApi.exportSubjects()}
            >
              导出
            </Button>
            <Button icon={<ImportOutlined />}>导入</Button>
            <Button
              type="primary"
              icon={<PlusOutlined />}
              onClick={() => handleAdd()}
            >
              新增科目
            </Button>
          </Space>
        }
      >
        <Spin spinning={loading}>
          {error ? (
            <Alert
              type="error"
              message={error}
              showIcon
              action={
                <Button size="small" onClick={loadTree}>
                  重试
                </Button>
              }
            />
          ) : treeData.length > 0 ? (
            <Tree
              showLine
              defaultExpandAll
              treeData={buildTreeNodes(treeData)}
              blockNode
            />
          ) : !loading ? (
            <Empty description="暂无科目数据，请先初始化" />
          ) : null}
        </Spin>
      </Card>
      <Modal
        title={editingRecord ? "编辑科目" : "新增科目"}
        open={modalOpen}
        onOk={handleSave}
        onCancel={() => setModalOpen(false)}
        width={600}
      >
        <Form form={form}>
          <Form.Item name="parentId" label="上级科目" hidden>
            <InputNumber />
          </Form.Item>
          <Form.Item
            name="subjectCode"
            label="科目编码"
            rules={[{ required: true }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="subjectName"
            label="科目名称"
            rules={[{ required: true }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="subjectType"
            label="科目类型"
            rules={[{ required: true }]}
          >
            <Select
              options={[
                { label: "资产", value: 1 },
                { label: "负债", value: 2 },
                { label: "权益", value: 3 },
                { label: "成本", value: 4 },
                { label: "损益", value: 5 },
              ]}
            />
          </Form.Item>
          <Form.Item
            name="balanceDirection"
            label="余额方向"
            rules={[{ required: true }]}
          >
            <Select
              options={[
                { label: "借方", value: 1 },
                { label: "贷方", value: 2 },
              ]}
            />
          </Form.Item>
          <Form.Item name="isEnabled" label="状态">
            <Select
              options={[
                { label: "启用", value: 1 },
                { label: "停用", value: 0 },
              ]}
            />
          </Form.Item>
          <Form.Item name="isCash" label="现金科目">
            <Select
              options={[
                { label: "否", value: 0 },
                { label: "是", value: 1 },
              ]}
            />
          </Form.Item>
          <Form.Item name="isBank" label="银行科目">
            <Select
              options={[
                { label: "否", value: 0 },
                { label: "是", value: 1 },
              ]}
            />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default SubjectList;
