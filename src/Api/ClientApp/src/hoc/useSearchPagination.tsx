import SearchBar from "@components/Ui/SearchBar";
import { Pagination, Select, UnstyledButton } from "@mantine/core";
// import { DatePickerInput } from "@mantine/dates";
import queryStringGenerator from "@utils/queryStringGenerator";
import React, { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { IconChevronDown, IconChevronUp, IconArrowsSort } from "@tabler/icons";

export interface IWithSearchPagination {
  searchParams: string;

  pagination: (totalPage: number, length: number) => JSX.Element;
  searchComponent: (placeholder?: string) => JSX.Element;
  sortComponent: (props: { title: string; sortKey: string }) => JSX.Element;
  filterComponent: (
    data: {
      value: string;
      label: string;
    }[],
    placeholder: string,
    key: string
  ) => JSX.Element;
  setInitialSearch: React.Dispatch<
    React.SetStateAction<
      {
        key: string;
        value: string;
      }[]
    >
  >;
}

const withSearchPagination =
  <P extends object>(Component: React.FC<P & IWithSearchPagination>) =>
  (props: P) => {
    const [params, setParams] = useSearchParams();
    const [sort, setSort] = useState(params.get("so") ?? "");
    const [filterKey, setFilterKey] = useState<string>("");
    const [initialSearch, setInitialSearch] = useState<
      {
        key: string;
        value: string;
      }[]
    >([
      {
        key: "",
        value: "",
      },
    ]);

    let search = params.get("s") ?? null;
    let pageSize = 12;
    const [itemLength, setItemLength] = useState<number>();
    const [filterValue, setFilterValue] = useState<string>("");
    const [currentPage, setCurrentPage] = useState(
      parseInt(params.get("p") ?? "1")
    );

    useEffect(() => {
      if (currentPage !== 1 && itemLength == 0) {
        setPage(currentPage - 1);
      }
    }, [itemLength]);
    const qs = useMemo(() => {
      const [by, type] = sort.split(":");
      const initSearchObject: Record<string, string> = {};
      initialSearch.forEach((x) => {
        initSearchObject[x.key] = x.value;
      });

      const qs = queryStringGenerator({
        ...initSearchObject,
        search,
        page: currentPage,
        size: pageSize,
        sortBy: by,
        sortType: type,
        [filterKey]: filterValue,
      });

      !!search && params.set("s", search);
      sort && params.set("so", sort);

      pageSize && params.set("si", pageSize?.toString());
      currentPage && params.set("p", currentPage.toString());

      setParams(params, { replace: true });
      return qs;
    }, [currentPage, search, pageSize, sort, filterValue, initialSearch]);

    const setSearch = (search: string) => {
      for (let value of params.entries()) {
        if (value[0] !== "s") params.delete(value[0]);
      }
      params.set("s", search);
      setParams(params);
    };

    const sortComponent = (props: { title: string; sortKey: string }) => {
      const sortKey = sort && sort.split(":").length > 0 && sort.split(":")[0];
      const sortValue =
        sort && sort.split(":").length > 0 && sort.split(":")[1];
      const isAscending = sortKey === props.sortKey && sortValue === "1";

      return (
        <UnstyledButton
          style={{
            display: "flex",
            alignItems: "center",
            fontWeight: "bold",
            color: "#495057",
            fontSize: "inherit",
          }}
          onClick={() => {
            setSort(() => props.sortKey + `:${!isAscending ? "1" : "0"}`);
          }}
        >
          {props.title}

          {props.sortKey === sortKey ? (
            isAscending ? (
              <IconChevronUp style={{ marginLeft: "10px" }} size={20} />
            ) : (
              <IconChevronDown style={{ marginLeft: "10px" }} size={20} />
            )
          ) : (
            <IconArrowsSort style={{ marginLeft: "10px" }} size={20} />
          )}
        </UnstyledButton>
      );
    };

    const filterComponent = (
      data: { value: string; label: string }[],
      placeholder: string,
      key: string
    ) => {
      return (
        <Select
          placeholder={placeholder}
          ml={5}
          clearable
          maw={"184px"}
          value={filterValue}
          data={data}
          onChange={(e: string) => {
            setFilterValue(() => e);
            setFilterKey(() => key);
          }}
        />
      );
    };

    const setPage = (pageNumber: number) => {
      setCurrentPage(pageNumber);
    };

    const pagination = (totalPage: number, length: number) => {
      setItemLength(length);
      return totalPage > 1 ? (
        <Pagination
          my={20}
          total={totalPage}
          page={currentPage}
          onChange={setPage}
        />
      ) : (
        <></>
      );
    };
    const searchComponent = (placeholder?: string) => (
      <SearchBar
        search={search ?? ""}
        setSearch={setSearch}
        placeholder={placeholder}
      />
    );

    const dateFilterComponent = (label: string, placeholder: string) => {
      <DatePickerInput />
    }

    return (
      <Component
        {...(props as P)}
        filterComponent={filterComponent}
        searchParams={qs}
        pagination={pagination}
        searchComponent={searchComponent}
        sortComponent={sortComponent}
        setInitialSearch={setInitialSearch}
      />
    );
  };

export default withSearchPagination;
