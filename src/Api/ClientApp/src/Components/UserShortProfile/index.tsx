import {
  Avatar,
  Box,
  Flex,
  MantineSize,
  StyleProp,
  Text,
  UnstyledButton,
} from '@mantine/core';
import { PoolRole, UserRole } from '@utils/enums';
import { getInitials } from '@utils/getInitialName';
import { IUser } from '@utils/services/types';
import { CSSProperties, FC } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

type Props = {
  user: IUser;
  size?: MantineSize | undefined;
  direction?: StyleProp<CSSProperties['flexDirection']>;
  page?: string;
  color?: string;
};

const UserShortProfile: FC<Props> = ({
  user: { fullName, id, imageUrl, role },
  size = 'xl',
  direction = undefined,
  page = 'Pool',
  color = '',
}) => {
  const { t } = useTranslation();
  return (
    <UnstyledButton
      component={Link}
      to={`/userProfile/${id}/about`}
      style={{ textDecoration: 'none' }}
    >
      <Flex direction={direction} gap={'md'} style={{ alignItems: 'center' }}>
        <Avatar my={3} src={imageUrl} c="cyan" radius={10000} size={size}>
          {!imageUrl && getInitials(fullName ?? '')}
        </Avatar>
        <Box>
          <Text size={size} fw="bolder" c={color}>
            {fullName}
          </Text>
          <Text size={'sm'} c={'dimmed'}>
            {page === 'Pool'
              ? t(`${PoolRole[Number(role)] ?? ''}`)
              : t(`${UserRole[Number(role)]}`)}
          </Text>
        </Box>
      </Flex>
    </UnstyledButton>
  );
};

export default UserShortProfile;
