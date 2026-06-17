/**
 * ProTable 通用 request 函数
 * 封装分页+搜索+排序的统一请求逻辑，供 @ant-design/pro-components 的 ProTable 使用
 */
import { message } from 'antd';
import type { ProTableParams } from '@ant-design/pro-components';
import type { PageParams, SearchParams, PagedResult } from '@/types/api.d';

/**
 * 创建 ProTable 的 request 函数
 * @param fetchData 业务数据获取函数
 */
export function createProTableRequest<T>(
  fetchData: (params: PageParams & SearchParams) => Promise<PagedResult<T>>
) {
  return async (params: ProTableParams & Record<string, unknown>): Promise<{ data: T[]; success: boolean; total: number }> => {
    const { current, pageSize, sortField, sortOrder, ...rest } = params;

    // 将 ProTable 的 current 映射为 pageIndex
    const requestParams: PageParams & SearchParams = {
      pageIndex: current ?? 1,
      pageSize: pageSize ?? 20,
    };

    // 排序参数（将 camelCase 转为 PascalCase 给后端）
    if (sortField) {
      requestParams.sortField = sortField
        .replace(/^[a-z]/, (c) => c.toUpperCase());
      requestParams.sortOrder = sortOrder === 'ascend' ? 'asc' : 'desc';
    }

    // 搜索条件（过滤掉空值）
    Object.entries(rest).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        requestParams[key] = value;
      }
    });

    try {
      const res = await fetchData(requestParams);
      const list = res.data?.list ?? [];
      return {
        data: list,
        success: true,
        total: res.data?.total ?? 0,
      };
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : '数据加载失败';
      console.error('[ProTable request error]', msg);
      message.error(`数据加载失败: ${msg}`);
      return {
        data: [],
        success: false,
        total: 0,
      };
    }
  };
}
