# Build Stage
FROM node:22-slim AS build

# Install pnpm
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN corepack enable

WORKDIR /src

# Copy package.json, lock file and workspace config
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./

# Install dependencies (ignoring scripts during install to avoid issues with pnpm-workspace.yaml allowBuilds)
RUN pnpm install --ignore-scripts

# Copy everything else
COPY . .

# Now allow scripts to run for the build
RUN pnpm run build

# Runtime Stage
FROM node:22-slim AS final

WORKDIR /app

# Copy output from build stage
COPY --from=build /src/.output ./.output

ENV HOST=0.0.0.0
ENV PORT=3000

EXPOSE 3000

CMD ["node", ".output/server/index.mjs"]
